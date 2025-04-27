using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PriceAlertsController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public PriceAlertsController(AppDbContext context)
        {
            _context = context;
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
        public async Task<ActionResult<IEnumerable<PriceAlertDto>>> GetAlerts()
        {
            var userId = GetUserId();
            var alerts = await _context.PriceAlerts
                .Include(a => a.Stock)
                .Where(a => a.UserId == userId)
                .Select(a => new PriceAlertDto
                {
                    Id = a.Id,
                    StockId = a.StockId,
                    StockSymbol = a.Stock != null ? a.Stock.Symbol : string.Empty,
                    StockName = a.Stock != null ? a.Stock.Name : string.Empty,
                    TargetPrice = a.TargetPrice,
                    IsAboveTarget = a.IsAboveTarget,
                    IsTriggered = a.IsTriggered,
                    CreatedAt = a.CreatedAt
                })
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
                
            return alerts;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<PriceAlertDto>> GetAlert(int id)
        {
            var userId = GetUserId();
            var alert = await _context.PriceAlerts
                .Include(a => a.Stock)
                .Where(a => a.Id == id && a.UserId == userId)
                .Select(a => new PriceAlertDto
                {
                    Id = a.Id,
                    StockId = a.StockId,
                    StockSymbol = a.Stock != null ? a.Stock.Symbol : string.Empty,
                    StockName = a.Stock != null ? a.Stock.Name : string.Empty,
                    TargetPrice = a.TargetPrice,
                    IsAboveTarget = a.IsAboveTarget,
                    IsTriggered = a.IsTriggered,
                    CreatedAt = a.CreatedAt
                })
                .FirstOrDefaultAsync();
                
            if (alert == null)
            {
                return NotFound();
            }
            
            return alert;
        }
        
        [HttpPost]
        public async Task<ActionResult<PriceAlertDto>> CreateAlert(CreatePriceAlertDto alertDto)
        {
            var userId = GetUserId();
            
            var stock = await _context.Stocks.FindAsync(alertDto.StockId);
            if (stock == null)
            {
                return BadRequest("Invalid stock ID");
            }
            
            var alert = new PriceAlert
            {
                StockId = alertDto.StockId,
                UserId = userId,
                TargetPrice = alertDto.TargetPrice,
                IsAboveTarget = alertDto.IsAboveTarget,
                IsTriggered = false,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.PriceAlerts.Add(alert);
            await _context.SaveChangesAsync();
            
            var response = new PriceAlertDto
            {
                Id = alert.Id,
                StockId = alert.StockId,
                StockSymbol = stock.Symbol,
                StockName = stock.Name,
                TargetPrice = alert.TargetPrice,
                IsAboveTarget = alert.IsAboveTarget,
                IsTriggered = alert.IsTriggered,
                CreatedAt = alert.CreatedAt
            };
            
            return CreatedAtAction(nameof(GetAlert), new { id = alert.Id }, response);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlert(int id)
        {
            var userId = GetUserId();
            var alert = await _context.PriceAlerts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            
            if (alert == null)
            {
                return NotFound();
            }
            
            _context.PriceAlerts.Remove(alert);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}