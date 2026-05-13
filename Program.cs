using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using nutrition_app_backend.Data;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Services.Auth;

using nutrition_app_backend.Services.Token;
using nutrition_app_backend.Services.User;
using nutrition_app_backend.Services.Streaks;
using nutrition_app_backend.Services.Subscriptions;
using Hangfire;
using Hangfire.MySql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Title = "Request failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Validation failed.",
                Instance = context.HttpContext.Request.Path.ToString()
            };

            return new BadRequestObjectResult(problemDetails);
        };
    });
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// ====== DB CONFIG ======
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<WaoDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

// =====================
// SERVICES / INTERFACES
// =====================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<nutrition_app_backend.Services.Foods.IFoodService, nutrition_app_backend.Services.Foods.FoodService>();

// Phase 3
builder.Services.AddScoped<IStreakService, StreakService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

// Hangfire Config
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage(
        new Hangfire.MySql.MySqlStorage(
            connectionString,
            new Hangfire.MySql.MySqlStorageOptions
            {
                TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                QueuePollInterval = TimeSpan.FromSeconds(15),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                PrepareSchemaIfNecessary = true,
                DashboardJobListLimit = 50000,
                TransactionTimeout = TimeSpan.FromMinutes(1),
                TablesPrefix = "Hangfire"
            }
        )
    )
);
builder.Services.AddHangfireServer();

// =====================
// AUTOMAPPER
// =====================
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// =====================
// AUTHENTICATION (JWT)
// =====================
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
           
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)) 
        };
        options.Events = new JwtBearerEvents
        {
            // ❌ Không có token hoặc không gửi Authorization header
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;

                return context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Request failed",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "Unauthorized",
                    Instance = context.Request.Path.ToString()
                });
            },

            // ❌ Token sai / expired / signature fail
            OnAuthenticationFailed = context =>
            {
                context.NoResult();
                context.Response.StatusCode = 401;

                return context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Request failed",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "Unauthorized",
                    Instance = context.Request.Path.ToString()
                });
            },

            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;

                return context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Request failed",
                    Status = StatusCodes.Status403Forbidden,
                    Detail = "Forbidden",
                    Instance = context.Request.Path.ToString()
                });
            }
        };
    });
builder.Services.AddAuthorization();


// ====== SWAGGER ======
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Nutrition API", Version = "v1" });

    // 1. Define the security scheme (Bearer token)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token (only the token string is needed)"
    });
    // 2. Make it global for all endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ====== MIDDLEWARE ======
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard (in dev mode, or secured in prod)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}

app.MapControllers();

// Hangfire Job Registration
RecurringJob.AddOrUpdate<nutrition_app_backend.Services.BackgroundTasks.StreakCronJob>(
    "daily-streak-process",
    job => job.ProcessDailyStreaks(),
    "59 23 * * *" // 23:59 everyday
);

// ====== API TEST DB ======
app.MapGet("/api/health/db", async (WaoDbContext dbContext) =>
{
    var canConnect = await dbContext.Database.CanConnectAsync();
    return canConnect
        ? Results.Ok("Database connection successful!")
        : Results.Problem(
            title: "Request failed",
            detail: app.Environment.IsDevelopment()
                ? "Database connection failed."
                : "Unexpected error occurred",
            statusCode: StatusCodes.Status500InternalServerError,
            instance: "/api/health/db");
});

app.Run();