namespace backend.DTOs
{
    public class CreateTradeDto
    {
        public int StockId { get; set; }
        public decimal EntryPrice { get; set; }
        public bool IsHolding { get; set; } = true;
    }
}