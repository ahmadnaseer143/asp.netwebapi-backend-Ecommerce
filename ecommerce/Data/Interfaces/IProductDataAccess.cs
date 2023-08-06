using ecommerce.Models;

namespace ecommerce.Data.Interfaces
{
  public interface IProductDataAccess
  {
    Task<List<Product>> GetProducts(string category, string subCategory, int count);

    Task<byte[]> GetProductImage(int productId);

    Task<List<Product>> GetAllProducts();

    Product GetProduct(int id);

    Task<Product> UpdateProduct(Product product);

    Task<int> InsertProduct(Product product);

    Task<bool> DeleteProduct(int id, string category, string subCategory);
  }
}
