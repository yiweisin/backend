using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<TradeHistory> TradeHistories { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (tableName != null)
                {
                    entity.SetTableName(tableName.ToLower());
                }
                
                foreach (var property in entity.GetProperties())
                {
                    var columnName = property.GetColumnName();
                    if (columnName != null)
                    {
                        property.SetColumnName(columnName.ToLower());
                    }
                }
            }
            
            modelBuilder.Entity<TradeHistory>()
                .HasOne(t => t.Stock)
                .WithMany()
                .HasForeignKey(t => t.StockId);
                
            modelBuilder.Entity<TradeHistory>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId);
                
            modelBuilder.Entity<Stock>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<TradeHistory>()
                .Property(t => t.EntryPrice)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<TradeHistory>()
                .Property(t => t.PNL)
                .HasColumnType("decimal(18,2)");
            
            //SEEDS
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "demo", 
                    PasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8" 
                }
            );
            
            var stocks = new List<Stock>
            {
                new Stock { Id = 1, Symbol = "AAPL", Name = "Apple Inc.", Price = 187.41m, Description = "Technology company that designs, manufactures, and markets smartphones, personal computers, tablets, wearables, and accessories." },
                new Stock { Id = 2, Symbol = "MSFT", Name = "Microsoft Corporation", Price = 403.78m, Description = "Technology company that develops, licenses, and supports software, services, devices, and solutions." },
                new Stock { Id = 3, Symbol = "AMZN", Name = "Amazon.com, Inc.", Price = 178.75m, Description = "Online retailer and web service provider." },
                new Stock { Id = 4, Symbol = "GOOGL", Name = "Alphabet Inc.", Price = 153.51m, Description = "Technology company that specializes in Internet-related services and products." },
                new Stock { Id = 5, Symbol = "META", Name = "Meta Platforms, Inc.", Price = 485.58m, Description = "Technology company that focuses on social media and technology." },
                new Stock { Id = 6, Symbol = "TSLA", Name = "Tesla, Inc.", Price = 175.34m, Description = "Electric vehicle and clean energy company." },
                new Stock { Id = 7, Symbol = "NVDA", Name = "NVIDIA Corporation", Price = 952.76m, Description = "Technology company that designs graphics processing units." },
                new Stock { Id = 8, Symbol = "JPM", Name = "JPMorgan Chase & Co.", Price = 198.47m, Description = "Financial services company." },
                new Stock { Id = 9, Symbol = "V", Name = "Visa Inc.", Price = 267.32m, Description = "Digital payments company." },
                new Stock { Id = 10, Symbol = "JNJ", Name = "Johnson & Johnson", Price = 153.92m, Description = "Pharmaceutical and medical devices company." },
                new Stock { Id = 11, Symbol = "WMT", Name = "Walmart Inc.", Price = 65.32m, Description = "Retail corporation that operates a chain of hypermarkets." },
                new Stock { Id = 12, Symbol = "PG", Name = "Procter & Gamble Co.", Price = 161.43m, Description = "Consumer goods corporation." },
                new Stock { Id = 13, Symbol = "BAC", Name = "Bank of America Corp.", Price = 39.25m, Description = "Multinational investment bank and financial services company." },
                new Stock { Id = 14, Symbol = "UNH", Name = "UnitedHealth Group Inc.", Price = 528.72m, Description = "Health insurance company." },
                new Stock { Id = 15, Symbol = "HD", Name = "Home Depot Inc.", Price = 343.17m, Description = "Home improvement retailer." },
                new Stock { Id = 16, Symbol = "XOM", Name = "Exxon Mobil Corp.", Price = 115.44m, Description = "Multinational oil and gas corporation." },
                new Stock { Id = 17, Symbol = "PFE", Name = "Pfizer Inc.", Price = 27.56m, Description = "Pharmaceutical corporation." },
                new Stock { Id = 18, Symbol = "AVGO", Name = "Broadcom Inc.", Price = 1256.83m, Description = "Designer, developer, and global supplier of semiconductor devices." },
                new Stock { Id = 19, Symbol = "CSCO", Name = "Cisco Systems, Inc.", Price = 48.90m, Description = "Technology company that develops networking hardware and software." },
                new Stock { Id = 20, Symbol = "VZ", Name = "Verizon Communications Inc.", Price = 41.75m, Description = "Telecommunications conglomerate." },
                new Stock { Id = 21, Symbol = "COST", Name = "Costco Wholesale Corp.", Price = 721.24m, Description = "Chain of membership-only warehouse clubs." },
                new Stock { Id = 22, Symbol = "CVX", Name = "Chevron Corp.", Price = 146.27m, Description = "Multinational energy corporation." },
                new Stock { Id = 23, Symbol = "ADBE", Name = "Adobe Inc.", Price = 525.31m, Description = "Multinational computer software company." },
                new Stock { Id = 24, Symbol = "CMCSA", Name = "Comcast Corp.", Price = 42.15m, Description = "Telecommunications conglomerate." },
                new Stock { Id = 25, Symbol = "NKE", Name = "Nike, Inc.", Price = 81.24m, Description = "Athletic footwear and apparel corporation." },
                new Stock { Id = 26, Symbol = "NFLX", Name = "Netflix, Inc.", Price = 678.69m, Description = "Streaming service and production company." },
                new Stock { Id = 27, Symbol = "PEP", Name = "PepsiCo, Inc.", Price = 174.81m, Description = "Food, snack, and beverage corporation." },
                new Stock { Id = 28, Symbol = "KO", Name = "Coca-Cola Co.", Price = 60.37m, Description = "Beverage corporation." },
                new Stock { Id = 29, Symbol = "ABT", Name = "Abbott Laboratories", Price = 108.25m, Description = "Medical devices and health care company." },
                new Stock { Id = 30, Symbol = "MRK", Name = "Merck & Co., Inc.", Price = 121.45m, Description = "Pharmaceutical company." }
            };
            
            modelBuilder.Entity<Stock>().HasData(stocks);
            modelBuilder.Entity<TradeHistory>().HasData(
                new TradeHistory { 
                    Id = 1, 
                    StockId = 1, 
                    UserId = 1, 
                    EntryPrice = 150.25m, 
                    PNL = 37.16m, 
                    Date = DateTime.SpecifyKind(new DateTime(2025, 2, 26), DateTimeKind.Utc),
                    IsHolding = true 
                },
                new TradeHistory { 
                    Id = 2, 
                    StockId = 2, 
                    UserId = 1, 
                    EntryPrice = 380.50m, 
                    PNL = 23.28m, 
                    Date = DateTime.SpecifyKind(new DateTime(2025, 3, 13), DateTimeKind.Utc),
                    IsHolding = true 
                },
                new TradeHistory { 
                    Id = 3, 
                    StockId = 3, 
                    UserId = 1, 
                    EntryPrice = 185.30m, 
                    PNL = -6.55m, 
                    Date = DateTime.SpecifyKind(new DateTime(2025, 3, 18), DateTimeKind.Utc),
                    IsHolding = false 
                }
            );
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
                
            base.OnConfiguring(optionsBuilder);
        }
    }
}