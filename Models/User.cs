// backend/Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        // New fields for notifications
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailNotificationsEnabled { get; set; } = false;
        public bool SmsNotificationsEnabled { get; set; } = false;
        public string? EmailSubscriptionArn { get; set; }
        public string? SmsSubscriptionArn { get; set; }
    }
}