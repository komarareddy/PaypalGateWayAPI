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
    }
}