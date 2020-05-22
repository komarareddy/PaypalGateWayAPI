1. Setting up the development tools : In order to work with paypal, they have given testing enivorment

The first thing you need is to create a PayPal developer account so that you can access this page 
https://developer.paypal.com/webapps/developer/applications/myapps. When you press on Sandbox accounts to the left of the page,
you will have the option to create two types of accounts: Personal or Business. 

Personal account will be used when you buy your product
Business account is the account that will receive the payment from the Personal account.

Seller account = Business account

Buyer account = Personal account

------------------------------------------------------------------------------
How a server integration works
1.Set up the payment
   Your buyer clicks the PayPal button.
   The PayPal button calls your server.
   Your server calls the PayPal API to set up the payment.
   The button launches the checkout flow in the buyer's browser

2.Execute the payment
  Your buyer clicks the Pay Now button in the PayPal Checkout flow.
  Buyer reviews their order and clicks the Agree & Pay button.
  The browser calls your server.
  Your server calls the PayPal API to execute the payment.
  You show a payment receipt page to the buyer.
-----------------------------------------------------------------------------
In ASP.Net Core WEB API
Step 1 : Instal PaypalCore SDk from NuGet Package 
Step 2 : Store clientId and clientSecret in Appsettings(you can get it from paypal website)
step 3 : Create Two API's End 1.CreatePayment 2.ExecutePayment
step 4 : Call 'CreatePayment' from front end
