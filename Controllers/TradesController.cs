using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TradesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;
        
        public TradesController(AppDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
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
        
        [HttpGet]
        public ActionResult<IEnumerable<TradeHistoryDto>> GetTrades()
        {
            var userId = GetUserId();
            var trades = _context.TradeHistories
                .Include(t => t.Stock)
                .Where(t => t.UserId == userId)
                .Select(t => new TradeHistoryDto
                {
                    Id = t.Id,
                    StockId = t.StockId,
                    StockSymbol = t.Stock != null ? t.Stock.Symbol : string.Empty,
                    StockName = t.Stock != null ? t.Stock.Name : string.Empty,
                    EntryPrice = t.EntryPrice,
                    PNL = t.PNL,
                    Date = t.Date,
                    IsHolding = t.IsHolding
                })
                .OrderByDescending(t => t.Date)
                .ToList();
                
            return trades;
        }
        
        [HttpGet("{id}")]
        public ActionResult<TradeHistoryDto> GetTrade(int id)
        {
            var userId = GetUserId();
            var trade = _context.TradeHistories
                .Include(t => t.Stock)
                .Where(t => t.Id == id && t.UserId == userId)
                .Select(t => new TradeHistoryDto
                {
                    Id = t.Id,
                    StockId = t.StockId,
                    StockSymbol = t.Stock != null ? t.Stock.Symbol : string.Empty,
                    StockName = t.Stock != null ? t.Stock.Name : string.Empty,
                    EntryPrice = t.EntryPrice,
                    PNL = t.PNL,
                    Date = t.Date,
                    IsHolding = t.IsHolding
                })
                .FirstOrDefault();
                
            if (trade == null)
            {
                return NotFound();
            }
            
            return trade;
        }
        
        [HttpPost]
        public async Task<ActionResult<TradeHistoryDto>> CreateTrade(CreateTradeDto tradeDto)
        {
            var userId = GetUserId();
            
            var stock = _context.Stocks.Find(tradeDto.StockId);
            if (stock == null)
            {
                return BadRequest("Invalid stock ID");
            }
            
            var trade = new TradeHistory
            {
                StockId = tradeDto.StockId,
                UserId = userId,
                EntryPrice = tradeDto.EntryPrice,
                PNL = 0, 
                Date = DateTime.UtcNow,
                IsHolding = tradeDto.IsHolding
            };
            
            _context.TradeHistories.Add(trade);
            await _context.SaveChangesAsync();
            
            _context.Entry(trade).Reference(t => t.Stock).Load();
            
            var response = new TradeHistoryDto
            {
                Id = trade.Id,
                StockId = trade.StockId,
                StockSymbol = trade.Stock != null ? trade.Stock.Symbol : string.Empty,
                StockName = trade.Stock != null ? trade.Stock.Name : string.Empty,
                EntryPrice = trade.EntryPrice,
                PNL = trade.PNL,
                Date = trade.Date,
                IsHolding = trade.IsHolding
            };
            
            var user = await _context.Users.FindAsync(userId);
            if (user != null && user.EmailNotificationsEnabled && !string.IsNullOrEmpty(user.Email))
            {
                string message = $"You have successfully purchased {trade.Stock?.Symbol} at ${trade.EntryPrice}.";
                string subject = $"New Trade: {trade.Stock?.Symbol} Purchased";
                
                await _notificationService.PublishMessageAsync(message, subject, "trade");
            }
            
            return CreatedAtAction(nameof(GetTrade), new { id = trade.Id }, response);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrade(int id, UpdateTradeDto tradeDto)
        {
            var userId = GetUserId();
            var trade = await _context.TradeHistories
                .Include(t => t.Stock)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            
            if (trade == null)
            {
                return NotFound();
            }
            
            bool wasPreviouslyHolding = trade.IsHolding;
            
            trade.PNL = tradeDto.PNL;
            trade.IsHolding = tradeDto.IsHolding;
            
            _context.Entry(trade).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            // Send notification when a trade is sold (status changed from holding to not holding)
            if (wasPreviouslyHolding && !trade.IsHolding)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null && user.EmailNotificationsEnabled && !string.IsNullOrEmpty(user.Email))
                {
                    string profitOrLoss = trade.PNL >= 0 ? "profit" : "loss";
                    string message = $"You have sold {trade.Stock?.Symbol} with a {profitOrLoss} of ${Math.Abs(trade.PNL)}.";
                    string subject = $"Trade Closed: {trade.Stock?.Symbol} Sold";
                    
                    await _notificationService.PublishMessageAsync(message, subject, "trade");
                }
            }
            
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrade(int id)
        {
            var userId = GetUserId();
            var trade = await _context.TradeHistories.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            
            if (trade == null)
            {
                return NotFound();
            }
            
            _context.TradeHistories.Remove(trade);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}