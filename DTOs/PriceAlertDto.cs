namespace backend.DTOs
{
    public class PriceAlertDto
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public string StockSymbol { get; set; } = string.Empty;
        public string StockName { get; set; } = string.Empty;
        public decimal TargetPrice { get; set; }
        public bool IsAboveTarget { get; set; }
        public bool IsTriggered { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}