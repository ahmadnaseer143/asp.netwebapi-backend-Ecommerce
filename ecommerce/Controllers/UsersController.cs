using ecommerce.Data.Interfaces;
using ecommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    readonly IUserDataAccess dataAccess;
    private readonly string DateFormat;

    public UsersController(IUserDataAccess dataAccess, IConfiguration configuration)
    {
      this.dataAccess = dataAccess;
      DateFormat = configuration["Constants:DateFormat"];

    }


    [HttpPost("RegisterUser")]
    public async Task<IActionResult> RegisterUser([FromBody] User user)
    {
      user.CreatedAt = DateTime.Now.ToString(DateFormat);
      user.ModifiedAt = DateTime.Now.ToString(DateFormat);

      var result = await dataAccess.InsertUser(user);

      string? message;
      if (result) message = "Account Created";
      else message = "Email already taken";
      return Ok(message);
    }

    [HttpPost("LoginUser")]
    public async Task<IActionResult> LoginUser([FromBody] User user)
    {
      var token = await dataAccess.IsUserPresent(user.Email, user.Password);
      if (token == "") token = "invalid";
      return Ok(token);
    }

    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
      var users = await dataAccess.GetAllUsers();

      return Ok(users);
    }
  }
}
