namespace backend.Models
{
    public class TradeHistory
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public Stock? Stock { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal PNL { get; set; }
        public DateTime Date { get; set; }
        public bool IsHolding { get; set; }
    }
}
