namespace backend.DTOs
{
    public class TradeHistoryDto
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public string StockSymbol { get; set; } = string.Empty;
        public string StockName { get; set; } = string.Empty;
        public decimal EntryPrice { get; set; }
        public decimal PNL { get; set; }
        public DateTime Date { get; set; }
        public bool IsHolding { get; set; }
    }
}