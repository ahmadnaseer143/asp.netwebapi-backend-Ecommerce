using ecommerce.Data.Interfaces;
using ecommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProductsController : ControllerBase
  {
    readonly IProductDataAccess dataAccess;

    public ProductsController(IProductDataAccess dataAccess, IConfiguration configuration)
    {
      this.dataAccess = dataAccess;
    }

    [HttpGet("GetProducts")]

    public async Task<IActionResult> GetProducts(string category, string subCategory, int count)
    {
      var result = await dataAccess.GetProducts(category, subCategory, count);
      return Ok(result);
    }

    [HttpGet("GetImage/{productId}")]
    public async Task<IActionResult> GetImage(int productId)
    {
      // Retrieve the image file path based on the productId
      byte[] imageBytes = await dataAccess.GetProductImage(productId);

      if (imageBytes == null || imageBytes.Length == 0)
      {
        return NotFound();
      }

      // Return the image file as the response with the appropriate content type
      return File(imageBytes, "image/jpg");
    }


    [HttpGet("GetAllProducts")]

    public async Task<IActionResult> GetAllProducts()
    {
      var result = await dataAccess.GetAllProducts();
      return Ok(result);
    }

    [HttpGet("GetProduct/{id}")]

    public IActionResult GetProduct(int id)
    {
      var result = dataAccess.GetProduct(id);
      return Ok(result);
    }

    [HttpPut("UpdateProduct")]

    public async Task<IActionResult> UpdateProduct(Product product)
    {
      var result = await dataAccess.UpdateProduct(product);
      if (result != null)
      {
        return Ok(result);
      }
      return BadRequest();
    }

    [HttpPost("InsertProduct")]
    public async Task<IActionResult> InsertProduct(Product product)
    {
      var boolValue = await dataAccess.InsertProduct(product);

      if (boolValue > 0)
      {
        return Ok(boolValue);
      }

      return BadRequest();
    }


    [HttpDelete("DeleteProduct/{id}")]

    public async Task<IActionResult> DeleteProduct(int id, string category, string subCategory)
    {
      var boolValue = await dataAccess.DeleteProduct(id, category, subCategory);
      if (boolValue == false)
      {
        return NotFound();
      }

      return Ok(true);
    }
  }
}
