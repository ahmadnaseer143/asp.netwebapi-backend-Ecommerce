using ecommerce.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CartsController : ControllerBase
  {
    readonly ICartDataAccess dataAccess;
    private readonly string DateFormat;

    public CartsController(ICartDataAccess dataAccess, IConfiguration configuration)
    {
      this.dataAccess = dataAccess;
      DateFormat = configuration["Constants:DateFormat"];

    }

    [HttpPost("InsertCartItem/{userid}/{productid}")]
    public async Task<IActionResult> InsertCartItem(int userid, int productid)
    {
      var result = await dataAccess.InsertCartItem(userid, productid);
      return Ok(result ? "inserted" : "not inserted");
    }

    [HttpPost("RemoveCartItem/{userid}/{productid}")]
    public async Task<IActionResult> RemoveCartItem(int userid, int productid)
    {
      var result = await dataAccess.RemoveCartItem(userid, productid);
      return Ok(result ? "removed" : "not removed");
    }




    [HttpGet("GetActiveCartOfUser/{id}")]
    public async Task<IActionResult> GetActiveCartOfUser(int id)
    {
      var result = await dataAccess.GetActiveCartOfUser(id);
      return Ok(result);
    }

    [HttpGet("GetAllPreviousCartsOfUser/{id}")]
    public async Task<IActionResult> GetAllPreviousCartsOfUser(int id)
    {
      var result = await dataAccess.GetAllPreviousCartsOfUser(id);
      return Ok(result);
    }
  }
}
