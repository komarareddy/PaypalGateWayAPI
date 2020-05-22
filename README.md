# PaypalGateWayAPI
Paypal Payment gateway provides Dot Net Core SDK, by using that SDK we can implement payment gateway in asp.net core WEB API

How a SDK integration works
1.Set up the payment

   Your buyer clicks the a button in HTML.
   
   The HTML button calls your server(Dot Net Core WEB API).
   
   Your server calls the PayPal API to set up the payment.
   
   The button launches the checkout flow in the buyer's browser

2.Execute the payment

  Your buyer clicks the Pay Now button in the PayPal Checkout flow.
  
  Buyer reviews their order and clicks the Agree & Pay button.
  
  The browser calls your server.
  
  Your server calls the PayPal API to execute the payment.
  
  You show a payment receipt page to the buyer.
  
  
  In ASP.Net Core WEB API
  
Step 1 : Instal PaypalCore SDk from NuGet Package 

Step 2 : Store clientId and clientSecret in Appsettings(you can get it from paypal website)

step 3 : Create Two API's End 1.CreatePayment 2.ExecutePayment

step 4 : Call 'CreatePayment' from front end
