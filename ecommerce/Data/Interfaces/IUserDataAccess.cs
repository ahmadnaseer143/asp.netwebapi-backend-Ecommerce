using ecommerce.Models;

namespace ecommerce.Data.Interfaces
{
  public interface IUserDataAccess
  {
    Task<bool> InsertUser(User user);
    Task<string> IsUserPresent(string email, string password);
    User GetUser(int id);

    Task<List<User>> GetAllUsers();
  }
}
