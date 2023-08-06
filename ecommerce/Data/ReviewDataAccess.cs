using ecommerce.Data.Interfaces;
using ecommerce.Models;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace ecommerce.Data
{
  public class ReviewDataAccess : IReviewDataAccess
  {
    private readonly IConfiguration configuration;
    private readonly string dbConnection;
    private readonly IProductDataAccess iProductDataAccess;
    private readonly IUserDataAccess iUserDataAccess;
    public ReviewDataAccess(IConfiguration configuration, IProductDataAccess iProductDataAccess, IUserDataAccess iUserDataAccess)
    {
      this.configuration = configuration;
      dbConnection = this.configuration.GetConnectionString("DB");
      this.iProductDataAccess = iProductDataAccess;
      this.iUserDataAccess = iUserDataAccess;
    }

    public async Task InsertReview(Review review)
    {
      using MySqlConnection connection = new(dbConnection);
      MySqlCommand command = new()
      {
        Connection = connection
      };

      string query = "INSERT INTO Reviews (UserId, ProductId, Review, CreatedAt) VALUES (@uid, @pid, @rv, @cat);";
      command.CommandText = query;
      command.Parameters.Add("@uid", MySqlDbType.Int32).Value = review.User.Id;
      command.Parameters.Add("@pid", MySqlDbType.Int32).Value = review.Product.Id;
      command.Parameters.Add("@rv", MySqlDbType.VarChar).Value = review.Value;
      command.Parameters.Add("@cat", MySqlDbType.VarChar).Value = review.CreatedAt;

      await connection.OpenAsync();
      command.ExecuteNonQuery();
    }


    public async Task<List<Review>> GetProductReviews(int productId)
    {
      var reviews = new List<Review>();
      using (MySqlConnection connection = new(dbConnection))
      {
        MySqlCommand command = new()
        {
          Connection = connection
        };

        string query = "SELECT * FROM Reviews WHERE ProductId = @productId;";
        command.CommandText = query;
        command.Parameters.AddWithValue("@productId", productId);

        await connection.OpenAsync();
        DbDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
          var review = new Review()
          {
            Id = (int)reader["ReviewId"],
            Value = (string)reader["Review"],
            CreatedAt = (string)reader["CreatedAt"]
          };

          var userId = (int)reader["UserId"];
          review.User = iUserDataAccess.GetUser(userId);

          var id = (int)reader["ProductId"];
          review.Product = iProductDataAccess.GetProduct(productId);

          reviews.Add(review);
        }
      }
      return reviews;
    }
  }
}
