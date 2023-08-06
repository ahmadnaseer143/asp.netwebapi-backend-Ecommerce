using ecommerce.Models;

namespace ecommerce.Data.Interfaces
{
  public interface ICategoryDataAccess
  {
    Task<List<ProductCategory>> GetProductCategories();

    Task<bool> InsertProductCategory(ProductCategory productCategory, IFormFile photoFile);

    Task<bool> UpdateCategory(ProductCategory category, IFormFile photoFile);
    Task<bool> DeleteProductCategory(int id);
    ProductCategory GetProductCategory(int id);

    Task<byte[]> GetBannerImage(string name);
  }
}
