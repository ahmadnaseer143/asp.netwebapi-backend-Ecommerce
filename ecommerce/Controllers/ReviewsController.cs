using ecommerce.Data.Interfaces;
using ecommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ReviewsController : ControllerBase
  {
    readonly IReviewDataAccess dataAccess;
    private readonly string DateFormat;

    public ReviewsController(IReviewDataAccess dataAccess, IConfiguration configuration)
    {
      this.dataAccess = dataAccess;
      DateFormat = configuration["Constants:DateFormat"];

    }

    [HttpPost("InsertReview")]
    public async Task<IActionResult> InsertReview([FromBody] Review review)
    {
      review.CreatedAt = DateTime.Now.ToString(DateFormat);
      dataAccess.InsertReview(review);
      return Ok("Review inserted");
    }

    [HttpGet("GetProductReviews/{productId}")]
    public async Task<IActionResult> GetProductReviews(int productId)
    {
      var result = await dataAccess.GetProductReviews(productId);
      return Ok(result);
    }
  }
}
