using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.DTOs;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public StocksController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<StockDto>> GetStocks()
        {
            var stocks = _context.Stocks
                .Select(s => new StockDto
                {
                    Id = s.Id,
                    Symbol = s.Symbol,
                    Name = s.Name,
                    Price = s.Price,
                    Description = s.Description
                })
                .ToList();
                
            return stocks;
        }
        
        [HttpGet("{id}")]
        public ActionResult<StockDto> GetStock(int id)
        {
            var stock = _context.Stocks
                .Where(s => s.Id == id)
                .Select(s => new StockDto
                {
                    Id = s.Id,
                    Symbol = s.Symbol,
                    Name = s.Name,
                    Price = s.Price,
                    Description = s.Description
                })
                .FirstOrDefault();
                
            if (stock == null)
            {
                return NotFound();
            }
            
            return stock;
        }
        
        [HttpGet("prices")]
        public ActionResult<IEnumerable<object>> GetStockPrices()
        {
            var stockPrices = _context.Stocks
                .Select(s => new 
                {
                    s.Id,
                    s.Symbol,
                    s.Price
                })
                .ToList();
                
            return stockPrices;
        }
        
        [HttpGet("{id}/history")]
        public ActionResult<IEnumerable<object>> GetPriceHistory(int id)
        {
            var stock = _context.Stocks.Find(id);
            if (stock == null)
            {
                return NotFound();
            }
            
            var random = new Random();
            var currentPrice = stock.Price;
            var history = new List<object>();
            var baseDate = DateTime.Now.AddDays(-30);
            
            for (int i = 0; i < 30; i++)
            {
                var fluctuation = (decimal)(random.NextDouble() * 10 - 5) / 100;
                var historicalPrice = Math.Round(currentPrice * (1 - (i * 0.005m + fluctuation)), 2);
                
                if (historicalPrice < 1.00m)
                {
                    historicalPrice = 1.00m;
                }
                
                history.Add(new
                {
                    Date = baseDate.AddDays(i).ToString("yyyy-MM-dd"),
                    Price = historicalPrice
                });
            }
            
            return history;
        }
    }
}