namespace ecommerce.Models
{
  public class EditCategoryRequest
  {
    public ProductCategory Category { get; set; }
    public IFormFile PhotoFile { get; set; }
  }
}
