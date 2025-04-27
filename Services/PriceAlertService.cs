using backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace backend.Services
{
    public class PriceAlertService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        
        public PriceAlertService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckPriceAlerts();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PriceAlertService: {ex.Message}");
                }
                
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        
        private async Task CheckPriceAlerts()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
            
            var activeAlerts = await context.PriceAlerts
                .Include(a => a.Stock)
                .Include(a => a.User)
                .Where(a => !a.IsTriggered)
                .ToListAsync();
                
            foreach (var alert in activeAlerts)
            {
                bool shouldTrigger = false;
                
                if (alert.IsAboveTarget && alert.Stock.Price >= alert.TargetPrice)
                {
                    shouldTrigger = true;
                }
                else if (!alert.IsAboveTarget && alert.Stock.Price <= alert.TargetPrice)
                {
                    shouldTrigger = true;
                }
                
                if (shouldTrigger)
                {
                    alert.IsTriggered = true;
                    await context.SaveChangesAsync();
                    

                    if (alert.User.EmailNotificationsEnabled && !string.IsNullOrEmpty(alert.User.Email))
                    {
                        string direction = alert.IsAboveTarget ? "above" : "below";
                        string message = $"Price alert triggered! {alert.Stock.Symbol} is now {direction} ${alert.TargetPrice}. Current price: ${alert.Stock.Price}";
                        string subject = $"Price Alert: {alert.Stock.Symbol} - Target Reached";
                        
                        try
                        {
                            await notificationService.PublishMessageAsync(message, subject, "alert");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to send price alert notification: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}