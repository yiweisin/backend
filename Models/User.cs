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
    }
}