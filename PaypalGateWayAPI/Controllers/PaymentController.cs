using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using PaypalGateWayAPI.PayPalService;

namespace PaypalGateWayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class PaymentController : ControllerBase
    {
        private readonly IPaypalPaymentService _paypalPaymentService;
        public PaymentController(IPaypalPaymentService paypalPaymentService)
        {
            _paypalPaymentService = paypalPaymentService;
        }
        /// <summary>
        /// Create A new payment resource
        /// </summary>
        /// <returns></returns>
        /// <returns>A response with new customer</returns>
        /// <response code="201">A response as creation of customer</response>
        /// <response code="400">For bad request</response>
        /// <response code="500">If there was an internal server error</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("CreatePayment")]
      
        public IActionResult CreatePayment([FromBody]PaymentViewModel paymentViewModel)
        {
           var result = _paypalPaymentService.CreatePayment(paymentViewModel);

            foreach (var link in result.links)
            {
                if (link.rel.ToLower().Trim().Equals("approval_url"))
                {
                    // return Redirect(link.href);
                    return Content(link.href);
                   // return json(new { url = link.href });
                }
            }

            return NotFound("Error in payment");
        }

        [HttpGet()]
        [Route("success")]
        public IActionResult ExecutePayment(string paymentId, string token, string PayerID)
        {
           var result =  _paypalPaymentService.ExecutePayment(PayerID, paymentId);
            return Ok(result);
        }

        [HttpGet("cancel")]
        public IActionResult CancelPayment(string token)
        {
            return Ok("Payment Not Successfull");
        }


        /// <summary>
        /// paypal payment with credit card
        /// </summary>
        /// <param name="paymentViewModel"></param>
        /// <param name="creditCard"></param>
        /// <returns></returns>
        /// <response code="201">A response as creation of customer</response>
        /// <response code="400">For bad request</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost()]
        [Route("api/PaymentWithCreditCard")]
        public IActionResult PaymentWithCreditCard([FromBody]PaymentViewModelForCreditCard paymentViewModel)
        {
            Payment createdPayment = new Payment();
            try
            {
                //Now make a List of Item and add the above item to it
                //you can create as many items as you want and add to this list
                List<Item> itms = new List<Item>();

                //create and item for which you are taking payment
                //if you need to add more items in the list
                //Then you will need to create multiple item objects or use some loop to instantiate object
                //loop through each item or produce
                Item item = new Item();
                foreach (var item1 in paymentViewModel.items)
                {
                    item.currency = "INR";
                    item.name = item1.Name;
                    item.description = item1.Description;
                    item.price = item1.Price.ToString();
                    item.quantity = item1.Quantity.ToString();
                    item.sku = "sku";

                    itms.Add(item);
                }


                //Item item = new Item();
                //item.name = "Item Name";
                //item.currency = "USD";
                //item.price = "1";
                //item.quantity = "5";
                //item.sku = "sku";



                ItemList itemList = new ItemList();
                itemList.items = itms;

                //Address for the payment
                PayPal.Api.Address billingAddress = new PayPal.Api.Address();
                billingAddress.city = paymentViewModel.shipping_Address.City;
                billingAddress.country_code = paymentViewModel.shipping_Address.Country_Code;
                billingAddress.line1 = paymentViewModel.shipping_Address.Line1;
                billingAddress.postal_code = paymentViewModel.shipping_Address.postal_code;
                billingAddress.state = paymentViewModel.shipping_Address.State;

                //Now Create an object of credit card and add above details to it
                //Please replace your credit card details over here which you got from paypal
                CreditCard crdtCard = new CreditCard();
                crdtCard.billing_address = billingAddress;
                crdtCard.cvv2 = paymentViewModel.creditCardDetails.CVV;//"272";  //card cvv2 number
                crdtCard.expire_month = paymentViewModel.creditCardDetails.Expire_Month; //09; //card expire date
                crdtCard.expire_year = paymentViewModel.creditCardDetails.Expire_Year;//2022; //card expire year
                crdtCard.first_name = paymentViewModel.creditCardDetails.FirstName; //"John";
                crdtCard.last_name = paymentViewModel.creditCardDetails.LastName; //"Cena";
                crdtCard.number = paymentViewModel.creditCardDetails.CreditCardNumber; //"4229047773305741"; //enter your credit card number here
                crdtCard.type = paymentViewModel.creditCardDetails.CreditCardType; // "visa"; //credit card type here paypal allows 4 types

                // Specify details of your payment amount.
                Details details = new Details();
                details.shipping = paymentViewModel.ShippingCharge.ToString();
                details.subtotal = paymentViewModel.SubTotal.ToString();
                details.tax = paymentViewModel.Tax.ToString();

                // Specify your total payment amount and assign the details object
                Amount amnt = new Amount();
                amnt.currency = paymentViewModel.Currency;
                // Total = shipping tax + subtotal.
                amnt.total = Convert.ToString(paymentViewModel.SubTotal + paymentViewModel.ShippingCharge + paymentViewModel.Tax); ;
                amnt.details = details;

                // Now make a transaction object and assign the Amount object
                Transaction tran = new Transaction();
                tran.amount = amnt;
                tran.description = "Description about the payment amount.";
                tran.item_list = itemList;
                tran.invoice_number = "your invoice number which you are generating";

                // Now, we have to make a list of transaction and add the transactions object
                // to this list. You can create one or more object as per your requirements

                List<Transaction> transactions = new List<Transaction>();
                transactions.Add(tran);

                // Now we need to specify the FundingInstrument of the Payer
                // for credit card payments, set the CreditCard which we made above

                FundingInstrument fundInstrument = new FundingInstrument();
                fundInstrument.credit_card = crdtCard;

                // The Payment creation API requires a list of FundingIntrument

                List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
                fundingInstrumentList.Add(fundInstrument);

                // Now create Payer object and assign the fundinginstrument list to the object
                Payer payr = new Payer();
                payr.funding_instruments = fundingInstrumentList;
                payr.payment_method = "credit_card";

                // finally create the payment object and assign the payer object & transaction list to it
                Payment pymnt = new Payment();
                pymnt.intent = "sale";
                pymnt.payer = payr;
                pymnt.transactions = transactions;

                try
                {
                    //getting context from the paypal
                    //basically we are sending the clientID and clientSecret key in this function
                    //to the get the context from the paypal API to make the payment
                    //for which we have created the object above.

                    //Basically, apiContext object has a accesstoken which is sent by the paypal
                    //to authenticate the payment to facilitator account.
                    //An access token could be an alphanumeric string

                    // Get a reference to the config
                    var config = ConfigManager.Instance.GetProperties();

                    // Use OAuthTokenCredential to request an access token from PayPal
                    var accessToken = new OAuthTokenCredential(config).GetAccessToken();
                    var apiContext = new APIContext(accessToken);

                    //Create is a Payment class function which actually sends the payment details
                    //to the paypal API for the payment. The function is passed with the ApiContext
                    //which we received above.

                    createdPayment = pymnt.Create(apiContext);

                    //if the createdPayment.state is "approved" it means the payment was successful else not

                    //if (createdPayment.state.ToLower() != "approved")
                    //{
                    //    return createdPayment;
                    //}
                }
                catch (PayPal.PayPalException e)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (createdPayment.state == "approved")
            {
                return Ok("payment was successful");
            }
            else
            {
                return Ok("payment was not successful");
            }
        }

      
    }
}