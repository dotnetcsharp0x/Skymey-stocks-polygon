using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Skymey_main_lib.Models.Prices.Polygon;
using Skymey_main_lib.Models.Tickers.Polygon;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Skymey_stocks_polygon.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Ticker> Ticker { get; init; }
        public DbSet<TickerList> TickerList { get; init; }
        public static ApplicationContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<ApplicationContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Ticker>().ToCollection("stock_actual_prices");
            modelBuilder.Entity<TickerList>().ToCollection("stock_tickerlist");
        }
    }
}
