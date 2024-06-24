using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Synapse.Orders.Services;

namespace Synapse.Orders.Models
{
    public class OrderProcessor
    {
        private readonly IOrderService _orderService;
        private readonly IAlertService _alertService;
        private readonly ILogger<OrderProcessor> _logger;

        public OrderProcessor(IOrderService orderService, IAlertService alertService, ILogger<OrderProcessor> logger)
        {
            _orderService = orderService;
            _alertService = alertService;
            _logger = logger;
        }

        public async Task ProcessOrdersAsync()
        {
            _logger.LogInformation("Start processing orders.");
            var orders = await _orderService.FetchMedicalEquipmentOrdersAsync();
            foreach (var order in orders)
            {
                var updatedOrder = _orderService.ProcessOrder(order);
                await _alertService.SendAlertAndUpdateOrderAsync(updatedOrder);
            }
            _logger.LogInformation("Finished processing orders.");
        }
    }
}
