using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using RestSharp;
using Skymey_stocks_polygon.Actions.Prices;
using Skymey_stocks_polygon.Data;

namespace Skymey_stocks_polygon
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddSingleton<IHostedService, MySpecialService>();
                });
            await builder.RunConsoleAsync();
        }
    }

    public class MySpecialService : BackgroundService
    {
        GetPrices gp = new GetPrices();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    gp.GetPricesFromPolygon();
                    await Task.Delay(TimeSpan.FromSeconds(20));
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
