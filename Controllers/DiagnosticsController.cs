using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Models.Notifications;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/diagnostics")]
public class DiagnosticsController : ControllerBase
{
    private readonly WaoDbContext _context;

    public DiagnosticsController(WaoDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Ping endpoint to measure network latency and get server status
    /// </summary>
    [HttpGet("ping")]
    [AllowAnonymous]
    public IActionResult Ping()
    {
        return Ok(new
        {
            Status = "OK",
            Message = "pong",
            ServerTime = DateTime.UtcNow,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// Verify database connectivity by running a count query on Users
    /// </summary>
    [HttpGet("db-check")]
    [AllowAnonymous]
    public async Task<IActionResult> DbCheck()
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var userCount = await _context.Users.CountAsync();
            stopwatch.Stop();

            return Ok(new
            {
                Status = "Healthy",
                Database = "Microsoft SQL Server / LocalDB",
                LatencyMs = stopwatch.ElapsedMilliseconds,
                UserCount = userCount,
                Message = "Database connection established successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Status = "Unhealthy",
                Message = "Failed to connect to the database.",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Fetch server diagnostic metrics and database entity counts
    /// </summary>
    [HttpGet("system-info")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSystemInfo()
    {
        try
        {
            var process = Process.GetCurrentProcess();
            
            // Collect database counts
            var foodCount = await _context.FoodItems.CountAsync();
            var categoryCount = await _context.FoodCategories.CountAsync();
            var exerciseCount = await _context.Exercises.CountAsync();
            var dailyPlanCount = await _context.DailyPlans.CountAsync();
            var streakCount = await _context.UserStreaks.CountAsync();

            return Ok(new
            {
                Environment = new
                {
                    OS = Environment.OSVersion.ToString(),
                    MachineName = Environment.MachineName,
                    ProcessorCount = Environment.ProcessorCount,
                    RuntimeVersion = Environment.Version.ToString(),
                    WorkingSetMB = process.WorkingSet64 / (1024 * 1024),
                    ThreadCount = process.Threads.Count
                },
                DatabaseStats = new
                {
                    TotalFoods = foodCount,
                    TotalFoodCategories = categoryCount,
                    TotalExercises = exerciseCount,
                    ActiveDailyPlans = dailyPlanCount,
                    TotalStreaks = streakCount
                },
                Message = "System diagnostic data fetched successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Status = "Error",
                Message = "Failed to fetch system diagnostic information.",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Trigger a mock alert notification in DB for current user to test sync
    /// </summary>
    [HttpPost("mock-alert")]
    [Authorize]
    public async Task<IActionResult> TriggerMockAlert()
    {
        try
        {
            var userId = User.GetUserId();
            
            // Fetch default notification type (DAILY_SUMMARY or general)
            var notifType = await _context.NotificationTypes.FirstOrDefaultAsync(nt => nt.Code == "DAILY_SUMMARY") 
                            ?? await _context.NotificationTypes.FirstOrDefaultAsync();

            if (notifType == null)
            {
                return BadRequest(new { Message = "No NotificationTypes seeded in database." });
            }

            // Create notification record
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                NotificationTypeId = notifType.Id,
                Title = "🛡️ Chẩn đoán hệ thống thành công!",
                Message = $"Kết nối mạng và cơ sở dữ liệu đã được xác minh thành công vào lúc {DateTime.Now:HH:mm:ss}!",
                Data = "{\"source\": \"diagnostics\"}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Mock alert triggered and saved to notification log.",
                NotificationId = notification.Id,
                Title = notification.Title
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Status = "Error",
                Message = "Failed to trigger mock notification.",
                Error = ex.Message
            });
        }
    }
}
