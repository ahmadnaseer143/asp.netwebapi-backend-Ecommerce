using ecommerce.Data.Interfaces;
using ecommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace ecommerce.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PaymentsController : ControllerBase
  {
    readonly IPaymentDataAccess dataAccess;
    private readonly string DateFormat;
    private readonly string StripeSecretKey;

    public PaymentsController(IPaymentDataAccess dataAccess, IConfiguration configuration)
    {
      this.dataAccess = dataAccess;
      DateFormat = configuration["Constants:DateFormat"];
      StripeSecretKey = configuration["Stripe:SecretKey"];

    }

    [HttpGet("GetPaymentMethods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
      var result = await dataAccess.GetPaymentMethods();
      return Ok(result);
    }

    [HttpPost("InsertPayment")]
    public async Task<IActionResult> InsertPayment(Payment payment)
    {
      payment.CreatedAt = DateTime.Now.ToString();
      var id = await dataAccess.InsertPayment(payment);
      return Ok(id.ToString());
    }

    [HttpPost("ProcessStripePayment")]
    public IActionResult ProcessStripePayment([FromBody] StripePaymentRequest request)
    {
      StripeConfiguration.ApiKey = StripeSecretKey;

      try
      {
        // Create a charge using the Stripe API
        var options = new ChargeCreateOptions
        {
          Source = request.StripeToken,
          Amount = request.Amount,
          Currency = "usd",
          Description = request.Description,
          Shipping = new ChargeShippingOptions
          {
            Name = "Jenny Rosen",
            Address = new AddressOptions
            {
              Line1 = "510 Townsend St",
              PostalCode = "98140",
              City = "San Francisco",
              State = "CA",
              Country = "US",
            },
          },
        };

        var service = new ChargeService();
        var charge = service.Create(options);

        if (charge.Status == "succeeded")
        {
          // Payment was successful, return a success response to the frontend
          return Ok(new { success = true, message = "Payment successful" });
        }
        else
        {
          // Payment failed, return an error response to the frontend
          return BadRequest(new { success = false, message = "Payment failed" });
        }
      }
      catch (StripeException ex)
      {
        // Handle any exceptions from Stripe API
        return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Payment failed: " + ex.Message });
      }
    }
  }
}
