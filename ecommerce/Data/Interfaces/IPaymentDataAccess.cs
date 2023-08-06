using ecommerce.Models;

namespace ecommerce.Data.Interfaces
{
  public interface IPaymentDataAccess
  {
    Task<List<PaymentMethod>> GetPaymentMethods();
    Task<int> InsertPayment(Payment payment);
  }
}
