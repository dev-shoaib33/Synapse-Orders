using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Synapse.Orders.Models;
using Synapse.Orders.Services;

namespace Synapse.OrdersExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var processor = host.Services.GetRequiredService<OrderProcessor>();
            await processor.ProcessOrdersAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services.AddHttpClient();
                    services.AddSingleton<IOrderService, OrderService>();
                    services.AddSingleton<IAlertService, AlertService>();
                    services.AddSingleton<OrderProcessor>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                });
    }
}
