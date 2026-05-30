using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using nutrition_app_backend.Data;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Services.Auth;
using nutrition_app_backend.Services.Food;
using nutrition_app_backend.Services.FoodLog;
using nutrition_app_backend.Services.Storage;
using nutrition_app_backend.Services.Token;
using nutrition_app_backend.Services.User;
using nutrition_app_backend.Services.WeightLog;
using nutrition_app_backend.Services.Exercise;
using nutrition_app_backend.Services.Notification;
using nutrition_app_backend.Services.Admin;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    })
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
            .AllowAnyOrigin() // Allow all origins for development
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
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IFoodLogService, FoodLogService>();
builder.Services.AddScoped<IWeightLogService, WeightLogService>();
builder.Services.AddScoped<IStorageService, CloudinaryStorageService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAdminFoodService, AdminFoodService>();
builder.Services.AddScoped<IAdminExerciseService, AdminExerciseService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<NotificationBackgroundService>();

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
           
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("JWT Key is not configured."))) 
        };
        options.Events = new JwtBearerEvents
        {
            // ❌ Không có token hoặc không gửi Authorization header
            OnChallenge = context =>
            {
                // Skip default logic
                context.HandleResponse();
                
                // Only set status if response hasn't started
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    
                    return context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Title = "Request failed",
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = "Unauthorized",
                        Instance = context.Request.Path.ToString()
                    });
                }
                
                return Task.CompletedTask;
            },

            // ❌ Token sai / expired / signature fail
            OnAuthenticationFailed = context =>
            {
                // Don't call NoResult() - it causes issues
                return Task.CompletedTask;
            },

            OnForbidden = context =>
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";

                    return context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Title = "Request failed",
                        Status = StatusCodes.Status403Forbidden,
                        Detail = "Forbidden",
                        Instance = context.Request.Path.ToString()
                    });
                }
                
                return Task.CompletedTask;
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
app.MapControllers();

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