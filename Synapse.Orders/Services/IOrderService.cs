using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Orders.Services
{
   
        public interface IOrderService
        {
            Task<JObject[]> FetchMedicalEquipmentOrdersAsync();
            JObject ProcessOrder(JObject order);
        }
    }

