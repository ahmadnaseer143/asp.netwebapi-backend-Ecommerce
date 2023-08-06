using ecommerce.Models;

namespace ecommerce.Data.Interfaces
{
  public interface IReviewDataAccess
  {
    Task InsertReview(Review review);

    Task<List<Review>> GetProductReviews(int productId);
  }
}
