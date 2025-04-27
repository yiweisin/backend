using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;
        
        public NotificationsController(
            AppDbContext context, 
            NotificationService notificationService,
            ILogger<NotificationsController> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }
        
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return int.Parse(userIdClaim);
        }
        
        [HttpGet("preferences")]
        public async Task<ActionResult<NotificationPreferencesDto>> GetPreferences()
        {
            try
            {
                var userId = GetUserId();
                var user = await _context.Users.FindAsync(userId);
                
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                
                return new NotificationPreferencesDto
                {
                    Email = user.Email,
                    EmailNotificationsEnabled = user.EmailNotificationsEnabled,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification preferences");
                return StatusCode(500, new { message = "Error retrieving notification preferences", details = ex.Message });
            }
        }
        
        [HttpPost("preferences")]
        public async Task<IActionResult> UpdatePreferences(NotificationPreferencesDto preferencesDto)
        {
            try
            {
                var userId = GetUserId();
                var user = await _context.Users.FindAsync(userId);
                
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                
                if (preferencesDto.Email != user.Email || 
                    preferencesDto.EmailNotificationsEnabled != user.EmailNotificationsEnabled)
                {
                    if (!string.IsNullOrEmpty(user.EmailSubscriptionArn) && user.EmailNotificationsEnabled)
                    {
                        try
                        {
                            await _notificationService.UnsubscribeEmailAsync(user.EmailSubscriptionArn);
                        }
                        catch (Exception ex)
                        {
            
                            _logger.LogWarning(ex, "Failed to unsubscribe old email");
                        }
                        
                        user.EmailSubscriptionArn = null;
                    }
                    
                    user.Email = preferencesDto.Email;
                    user.EmailNotificationsEnabled = preferencesDto.EmailNotificationsEnabled;
                    
                    if (user.EmailNotificationsEnabled && !string.IsNullOrEmpty(user.Email))
                    {
                        try
                        {
                            var subscriptionArn = await _notificationService.SubscribeEmailAsync(user.Email);
                            user.EmailSubscriptionArn = subscriptionArn;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to subscribe email");
                            return BadRequest(new { message = ex.Message });
                        }
                    }
                }
                
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification preferences");
                return StatusCode(500, new { message = "Error updating notification preferences", details = ex.Message });
            }
        }
        
        [HttpPost("test")]
        public async Task<IActionResult> SendTestNotification()
        {
            try
            {
                var userId = GetUserId();
                var user = await _context.Users.FindAsync(userId);
                
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                
                if (!user.EmailNotificationsEnabled)
                {
                    return BadRequest(new { message = "No notification methods are enabled" });
                }
                
                try
                {
                    await _notificationService.PublishMessageAsync(
                        "This is a test notification from your Trading App.",
                        "Trading App - Test Notification",
                        "test"
                    );
                    
                    return Ok(new { message = "Test notification sent successfully" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send test notification");
                    return StatusCode(500, new { message = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendTestNotification");
                return StatusCode(500, new { message = "Error sending test notification", details = ex.Message });
            }
        }
    }
}