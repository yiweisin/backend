using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class StockPriceService : BackgroundService
    {
        private readonly ILogger<StockPriceService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Random _random = new Random();
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(0.5);

        public StockPriceService(ILogger<StockPriceService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stock Price Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Updating stock prices at: {time}", DateTimeOffset.Now);
                
                try
                {
                    await UpdateStockPricesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating stock prices.");
                }

                await Task.Delay(_updateInterval, stoppingToken);
            }

            _logger.LogInformation("Stock Price Service is stopping.");
        }

        private async Task UpdateStockPricesAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var stocks = await dbContext.Stocks.ToListAsync();
                
                foreach (var stock in stocks)
                {
                    // Generate random percentage change between -3% and +3%
                    double percentageChange = (_random.NextDouble() * 6) - 3;
                    
                    // Calculate new price
                    decimal changeAmount = stock.Price * (decimal)(percentageChange / 100);
                    stock.Price += changeAmount;
                    
                    // Ensure price doesn't go below 1.00
                    if (stock.Price < 1.00m)
                    {
                        stock.Price = 1.00m;
                    }
                    
                    // Round to 2 decimal places
                    stock.Price = Math.Round(stock.Price, 2);
                    
                    _logger.LogInformation("Updated {symbol} price to {price}", stock.Symbol, stock.Price);
                }
                
                await dbContext.SaveChangesAsync();
            }
        }
    }
}