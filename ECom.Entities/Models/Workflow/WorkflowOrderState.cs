using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Entities.Models.Workflow
{
    public class WorkflowOrderState
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("orderId")]
        public Guid OrderId { get; set; }

        [JsonProperty("merchantStateId")]
        public int MerchantStateId { get; set; }
    }
}
