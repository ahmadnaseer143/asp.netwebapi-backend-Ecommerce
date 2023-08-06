using ecommerce.Models;

namespace ecommerce.Data.Interfaces
{
  public interface ICartDataAccess
  {
    Task<bool> InsertCartItem(int userId, int productId);

    Task<bool> RemoveCartItem(int userId, int productId);
    Task<Cart> GetActiveCartOfUser(int userid);
    Cart GetCart(int cartid);
    Task<List<Cart>> GetAllPreviousCartsOfUser(int userid);
  }
}
