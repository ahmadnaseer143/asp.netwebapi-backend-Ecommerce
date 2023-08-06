using ecommerce.Data.Interfaces;
using ecommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OffersController : ControllerBase
  {
    readonly IOfferDataAccess dataAccess;
    private readonly string DateFormat;

    public OffersController(IOfferDataAccess dataAccess, IConfiguration configuration)
    {
      this.dataAccess = dataAccess;
      DateFormat = configuration["Constants:DateFormat"];

    }

    [HttpGet("GetOffer/{id}")]
    public IActionResult GetOffer(int id)
    {
      var result = dataAccess.GetOffer(id);
      return Ok(result);
    }

    [HttpGet("GetAllOffers")]

    public async Task<IActionResult> GetAllOffers()
    {
      var result = await dataAccess.GetAllOffers();
      return Ok(result);
    }

    [HttpPost("InsertOffer")]
    public async Task<IActionResult> InsertOffer(Offer offer)
    {
      var result = await dataAccess.InsertOffer(offer);
      if (result)
      {
        return Ok(result);
      }
      else
      {
        return BadRequest("Failed to insert offer.");
      }
    }

  }
}
