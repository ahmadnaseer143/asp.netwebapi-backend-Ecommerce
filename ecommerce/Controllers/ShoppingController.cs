using ecommerce.Data;
using ecommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ecommerce.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ShoppingController : ControllerBase
  {
    readonly IDataAccess dataAccess;
    private readonly string DateFormat;

    public ShoppingController(IDataAccess dataAccess, IConfiguration configuration)
    {
      this.dataAccess = dataAccess;
      DateFormat = configuration["Constants:DateFormat"];

    }

  }
}
