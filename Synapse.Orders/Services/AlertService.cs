using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Orders.Services
{
        public class AlertService : IAlertService
        {
            private readonly HttpClient _httpClient;
            private readonly ILogger<AlertService> _logger;

            public AlertService(HttpClient httpClient, ILogger<AlertService> logger)
            {
                _httpClient = httpClient;
                _logger = logger;
            }

            public async Task SendAlertAndUpdateOrderAsync(JObject order)
            {
                var orderId = order["OrderId"].ToString();
                try
                {
                    foreach (var item in order["Items"])
                    {
                        if (item["Status"].ToString().Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                        {
                            await SendAlertMessageAsync(item, orderId);
                        }
                    }

                    await UpdateOrderAsync(order);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to process order {orderId}");
                }
            }

            private async Task SendAlertMessageAsync(JToken item, string orderId)
            {
                const string alertApiUrl = "http://localhost:3000/alerts";
                var alertData = new
                {
                    Message = $"Alert for delivered item: Order {orderId}, Item: {item["Description"]}, " +
                              $"Delivery Notifications: {item["deliveryNotification"]}"
                };
                var content = new StringContent(JObject.FromObject(alertData).ToString(), Encoding.UTF8, "application/json");

                try
                {
                    var response = await _httpClient.PostAsync(alertApiUrl, content);
                    response.EnsureSuccessStatusCode();
                    _logger.LogInformation($"Alert sent for delivered item: {item["Description"]}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send alert for delivered item: {item["Description"]}");
                }
            }

            private async Task UpdateOrderAsync(JObject order)
            {
                const string updateApiUrl = "http://localhost:3000/update";
                var content = new StringContent(order.ToString(), Encoding.UTF8, "application/json");

                try
                {
                    var response = await _httpClient.PostAsync(updateApiUrl, content);
                    response.EnsureSuccessStatusCode();
                    _logger.LogInformation($"Updated order sent for processing: OrderId {order["OrderId"]}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send updated order for processing: OrderId {order["OrderId"]}");
                }
            }
        }
    }
