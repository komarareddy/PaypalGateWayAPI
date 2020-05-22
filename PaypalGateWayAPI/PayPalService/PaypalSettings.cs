using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaypalGateWayAPI.PayPalService
{
    public class PaypalSettings
    {
        public string Mode { get; set; }
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
        public string Return_url { get; set; }
        public string Cancel_url { get; set; }
    }
}
