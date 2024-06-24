using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json.Linq;

namespace Synapse.Orders.Services
{
        public class OrderService : IOrderService
        {
            private readonly HttpClient _httpClient;
            private readonly ILogger<OrderService> _logger;

            public OrderService(HttpClient httpClient, ILogger<OrderService> logger)
            {
                _httpClient = httpClient;
                _logger = logger;
            }

            public async Task<JObject[]> FetchMedicalEquipmentOrdersAsync()
            {
            //const string ordersApiUrl = "https://orders-api.com/orders";
            const string ordersApiUrl = "http://localhost:3000/orders";

            try
                {
                    var response = await _httpClient.GetAsync(ordersApiUrl);
                    response.EnsureSuccessStatusCode();
                    var ordersData = await response.Content.ReadAsStringAsync();
                    return JArray.Parse(ordersData).ToObject<JObject[]>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch orders from API.");
                    return Array.Empty<JObject>();
                }
            }

            public JObject ProcessOrder(JObject order)
            {
                var items = order["Items"].ToObject<JArray>();
                foreach (var item in items)
                {
                if (IsItemDelivered(item))
                {
                   Console.WriteLine($"Item {item["Description"]} is delivered. Incrementing delivery notification.");

                    IncrementDeliveryNotification(item);
                   Console.WriteLine($"Delivery notification count: {item["deliveryNotification"]}");

                }
                order["Items"] = items;
            }
            return order;
            }

        private bool IsItemDelivered(JToken item)
        {
            return item["Status"].ToString().Equals("Delivered", StringComparison.OrdinalIgnoreCase);
        }

        private void IncrementDeliveryNotification(JToken item)
        {
            item["deliveryNotification"] = item["deliveryNotification"].Value<int>() + 1;
        }
    }
    }

