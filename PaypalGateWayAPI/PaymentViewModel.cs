using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaypalGateWayAPI
{
    public class PaymentViewModel
    {
       
        public List<Items> items { get; set; }
       public Shipping_Address shipping_Address { get; set; }
        public double Tax { get; set; }

        public int ShippingCharge { get; set; }

        public int SubTotal { get; set; }

        public string Currency { get; set; }

    }

    public class Items
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int Quantity { get; set; }

        public int Price { get; set; }

    }

    public class Shipping_Address
    {
        public string Recipient_Name { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string postal_code { get; set; }
        public string Phone { get; set; }
        public string State { get; set; }
    }
}
