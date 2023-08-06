using ecommerce.Models;

namespace ecommerce.Data.Interfaces
{
  public interface IOrderDataAccess
  {
    Task<int> InsertOrder(Order order);

    Task<List<Order>> GetAllOrders();
  }
}
