namespace backend.Models
{
    public class PriceAlert
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int StockId { get; set; }
        public Stock? Stock { get; set; }
        public decimal TargetPrice { get; set; }
        public bool IsAboveTarget { get; set; } 
        public bool IsTriggered { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}