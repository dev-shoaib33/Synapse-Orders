using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using Synapse.Orders.Services;
using Xunit;

namespace Synapse.OrdersExample.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<OrderService>> _loggerMock;

        public OrderServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _loggerMock = new Mock<ILogger<OrderService>>();
        }

        [Fact]
        public async Task FetchMedicalEquipmentOrdersAsync_ReturnsOrders_WhenApiCallIsSuccessful()
        {
            // Arrange
            var ordersJson = "[{\"OrderId\": \"1\", \"Items\": []}]";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(ordersJson)
            };
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var orderService = new OrderService(_httpClient, _loggerMock.Object);

            // Act
            var orders = await orderService.FetchMedicalEquipmentOrdersAsync();

            // Assert
            Assert.Single(orders);
            Assert.Equal("1", orders[0]["OrderId"].ToString());
        }

        [Fact]
        public async Task FetchMedicalEquipmentOrdersAsync_ReturnsEmptyArray_WhenApiCallFails()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var orderService = new OrderService(_httpClient, _loggerMock.Object);

            // Act
            var orders = await orderService.FetchMedicalEquipmentOrdersAsync();

            // Assert
            Assert.Empty(orders);
        }

        [Fact]
        public void ProcessOrder_IncrementsDeliveryNotification_ForDeliveredItems()
        {
            // Arrange
            var orderJson = @"{
                'OrderId': '1',
                'Items': [
                    {'Description': 'Item1', 'Status': 'Delivered', 'deliveryNotification': 0},
                    {'Description': 'Item2', 'Status': 'Pending', 'deliveryNotification': 0}
                ]
            }";
            var order = JObject.Parse(orderJson);
            var orderService = new OrderService(_httpClient, _loggerMock.Object);

            // Act
            var processedOrder = orderService.ProcessOrder(order);

            // Assert
            var items = processedOrder["Items"];
            Assert.Equal(1, items[0]["deliveryNotification"].Value<int>());
            Assert.Equal(0, items[1]["deliveryNotification"].Value<int>());
        }
    }
}
