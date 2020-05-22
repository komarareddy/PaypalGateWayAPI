using Microsoft.Extensions.Options;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaypalGateWayAPI.PayPalService
{

    public interface IPaypalPaymentService
    {
        public Payment CreatePayment(PaymentViewModel paymentViewModel);

        public Payment ExecutePayment(string PayerID, string paymentId);
    }

    public class PaypalPaymentService : IPaypalPaymentService
    {
        private readonly PaypalSettings _paypalSettings;
        string accessToken = string.Empty;
        public PaypalPaymentService(IOptions<PaypalSettings> jwtsettings)
        {
            _paypalSettings = jwtsettings.Value;
            var ClientId = _paypalSettings.ClientId;
            var Secret = _paypalSettings.ClientSecret;

            accessToken = new OAuthTokenCredential(ClientId, Secret, 
                new Dictionary<string, string>() { { "mode", "sandbox" } }).GetAccessToken();
        }
        public Payment CreatePayment(PaymentViewModel paymentViewModel)
        {
            var apiContext = new APIContext(accessToken);

            //Create a Payment Object 
            Payment payment = new Payment();
            Payer payer = new Payer();
            List<Transaction> transactions = new List<Transaction>();
            Transaction transaction = new Transaction();
            Amount amount = new Amount();
            Details details = new Details();
            ItemList itemList = new ItemList();
            List<Item> items = new List<Item>();
            Item item = new Item();
            RedirectUrls redirectUrls = new RedirectUrls();
            ShippingAddress shipping_Address = new ShippingAddress();

            //The source of the funds for this payment.Payment method is PayPal Wallet payment or bank direct debit.
            payer.payment_method = "paypal";

            //Total tax
            details.tax = paymentViewModel.Tax.ToString();
            //Shipping charge
            details.shipping = paymentViewModel.ShippingCharge.ToString();
            //Total Amount of products not included tax, shipping charge
            details.subtotal = paymentViewModel.SubTotal.ToString();

            //loop through each item or produce
            foreach (var item1 in paymentViewModel.items)
            {
                item.currency = paymentViewModel.Currency;
                item.name = item1.Name;
                item.description = item1.Description;
                item.price = item1.Price.ToString();
                item.quantity = item1.Quantity.ToString();
                item.sku = "sku";

                items.Add(item);
            }

            itemList.items = items;
            itemList.shipping_address = shipping_Address;

            shipping_Address.recipient_name = paymentViewModel.shipping_Address.Recipient_Name;
            shipping_Address.line1 = paymentViewModel.shipping_Address.Line1;
            shipping_Address.line2 = paymentViewModel.shipping_Address.Line2;
            shipping_Address.city = paymentViewModel.shipping_Address.City;
            shipping_Address.postal_code = paymentViewModel.shipping_Address.postal_code;
            shipping_Address.phone = paymentViewModel.shipping_Address.Phone;
            shipping_Address.state = paymentViewModel.shipping_Address.State;
            shipping_Address.country_code = "IN";

            amount.currency = paymentViewModel.Currency;
            amount.total = Convert.ToString(paymentViewModel.SubTotal + paymentViewModel.ShippingCharge + paymentViewModel.Tax);
            amount.details = details;

            transaction.amount = amount;
            transaction.item_list = itemList;

            transactions.Add(transaction);

            redirectUrls.return_url = _paypalSettings.Return_url;
            redirectUrls.cancel_url = _paypalSettings.Cancel_url;


            payment.intent = "sale";
            payment.payer = payer;
            payment.transactions = transactions;
            payment.redirect_urls = redirectUrls;

            var CreatedPayment = Payment.Create(apiContext, payment);

            return CreatedPayment;

        }

        public Payment ExecutePayment(string PayerID, string paymentId)
        {
            var apiContext = new APIContext(accessToken);

            PaymentExecution paymentExecution = new PaymentExecution() { payer_id = PayerID };

            Payment payment = new Payment() { id= paymentId };

            var paymentExecuted = payment.Execute(apiContext, paymentExecution);

            return paymentExecuted;
        }
    }
}
