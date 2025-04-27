namespace backend.DTOs
{
    public class CreatePriceAlertDto
    {
        public int StockId { get; set; }
        public decimal TargetPrice { get; set; }
        public bool IsAboveTarget { get; set; }
    }
}