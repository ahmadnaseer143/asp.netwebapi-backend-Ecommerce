using ecommerce.Data.Interfaces;
using ecommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OrdersController : ControllerBase
  {
    readonly IOrderDataAccess dataAccess;
    private readonly string DateFormat;

    public OrdersController(IOrderDataAccess dataAccess, IConfiguration configuration)
    {
      this.dataAccess = dataAccess;
      DateFormat = configuration["Constants:DateFormat"];

    }

    [HttpPost("InsertOrder")]
    public async Task<IActionResult> InsertOrder(Order order)
    {
      order.CreatedAt = DateTime.Now.ToString();
      var id = await dataAccess.InsertOrder(order);
      return Ok(id.ToString());
    }

    [HttpGet("GetAllOrders")]
    public async Task<IActionResult> GetAllOrders()
    {
      var orders = await dataAccess.GetAllOrders();
      if (orders == null || orders.Count == 0)
      {
        return NotFound();
      }

      return Ok(orders);
    }
  }
}
