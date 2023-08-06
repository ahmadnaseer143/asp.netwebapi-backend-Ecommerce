using ecommerce.Data.Interfaces;
using ecommerce.Models;
using MySql.Data.MySqlClient;

namespace ecommerce.Data
{
  public class CategoryDataAccess : ICategoryDataAccess
  {
    private readonly IConfiguration configuration;
    private readonly string dbConnection;
    public CategoryDataAccess(IConfiguration configuration)
    {
      this.configuration = configuration;
      dbConnection = this.configuration.GetConnectionString("DB");
    }

    public async Task<List<ProductCategory>> GetProductCategories()
    {
      var productCategories = new List<ProductCategory>();
      using (MySqlConnection connection = new(dbConnection))
      {
        MySqlCommand command = new()
        {
          Connection = connection,
        };
        string query = "Select * from productcategories;";
        command.CommandText = query;
        await connection.OpenAsync();
        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
        {
          while (reader.Read())
          {
            var category = new ProductCategory()
            {
              Id = (int)reader["CategoryId"],
              Category = (string)reader["Category"],
              SubCategory = (string)reader["SubCategory"]
            };
            productCategories.Add(category);
          }
        }
      }

      return productCategories;
    }

    public async Task<bool> InsertProductCategory(ProductCategory productCategory, IFormFile photoFile)
    {
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand
        {
          Connection = connection
        };

        string query = "INSERT INTO productcategories (Category, SubCategory) VALUES (@Category, @SubCategory)";
        command.CommandText = query;
        command.Parameters.AddWithValue("@Category", productCategory.Category);
        command.Parameters.AddWithValue("@SubCategory", productCategory.SubCategory);

        try
        {
          await connection.OpenAsync();

          // Save the photo file to the "Resources/Banner" folder using the subCategory name as the filename
          string fileName = productCategory.SubCategory + Path.GetExtension(photoFile.FileName);
          string imagePath = Path.Combine("Resources", "Banner", fileName);
          using (var stream = new FileStream(imagePath, FileMode.Create))
          {
            await photoFile.CopyToAsync(stream);
          }

          int rowsAffected = await command.ExecuteNonQueryAsync();
          return rowsAffected > 0;
        }
        catch (Exception ex)
        {
          // Handling any potential exceptions
          Console.WriteLine($"Error inserting product category: {ex.Message}");
          return false;
        }
      }
    }

    public async Task<bool> UpdateCategory(ProductCategory category, IFormFile photoFile)
    {
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand
        {
          Connection = connection
        };

        string query = "UPDATE productcategories SET Category = @Category, SubCategory = @SubCategory WHERE CategoryId = @CategoryId";
        command.CommandText = query;
        command.Parameters.AddWithValue("@CategoryId", category.Id);
        command.Parameters.AddWithValue("@Category", category.Category);
        command.Parameters.AddWithValue("@SubCategory", category.SubCategory);

        try
        {
          await connection.OpenAsync();

          // Retrieve the previous SubCategory associated with the CategoryId
          string previousSubCategory = GetPreviousSubCategory(connection, category.Id);

          int rowsAffected = await command.ExecuteNonQueryAsync();

          // If the SubCategory is updated and a new photo is provided, update the image file
          if (rowsAffected > 0 && !string.IsNullOrEmpty(category.SubCategory) && photoFile != null)
          {
            // Delete the old image associated with the previous SubCategory
            DeleteImage(previousSubCategory);

            // Save the new photo file to the "Resources/Banner" folder using the updated SubCategory name as the filename
            string fileName = category.SubCategory + Path.GetExtension(photoFile.FileName);
            string imagePath = Path.Combine("Resources", "Banner", fileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
              await photoFile.CopyToAsync(stream);
            }
          }

          return rowsAffected > 0;
        }
        catch (Exception ex)
        {
          // Handling any potential exceptions
          Console.WriteLine($"Error updating category: {ex.Message}");
          return false;
        }
      }
    }

    private string GetPreviousSubCategory(MySqlConnection connection, int categoryId)
    {
      string previousSubCategory = string.Empty;
      MySqlCommand command = new MySqlCommand
      {
        Connection = connection
      };

      string query = "SELECT SubCategory FROM productcategories WHERE CategoryId = @CategoryId";
      command.CommandText = query;
      command.Parameters.AddWithValue("@CategoryId", categoryId);

      using (MySqlDataReader reader = command.ExecuteReader())
      {
        if (reader.Read())
        {
          previousSubCategory = reader["SubCategory"].ToString();
        }
      }

      return previousSubCategory;
    }

    private void DeleteImage(string subCategory)
    {
      // Ensure the subCategory is not empty or null
      if (!string.IsNullOrEmpty(subCategory))
      {
        // Combine the subCategory name with all supported image extensions to look for the file
        List<string> supportedExtensions = new List<string> { ".png", ".jpeg", ".jpg", ".gif" };
        foreach (string extension in supportedExtensions)
        {
          string imagePath = Path.Combine("Resources", "Banner", subCategory + extension);
          if (File.Exists(imagePath))
          {
            File.Delete(imagePath);
          }
        }
      }
    }




    public async Task<bool> DeleteProductCategory(int id)
    {
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand
        {
          Connection = connection
        };

        // Retrieve the subCategory associated with the category to be deleted
        string subCategoryToDelete = GetSubCategoryToDelete(connection, id);

        // Delete products associated with the category
        string deleteProductsQuery = "DELETE FROM products WHERE CategoryId = @Id";
        command.CommandText = deleteProductsQuery;
        command.Parameters.AddWithValue("@Id", id);

        try
        {
          await connection.OpenAsync();
          await command.ExecuteNonQueryAsync();

          // Delete the product category
          string deleteCategoryQuery = "DELETE FROM productcategories WHERE CategoryId = @Id";
          command.CommandText = deleteCategoryQuery;
          int rowsAffected = await command.ExecuteNonQueryAsync();

          // Delete the corresponding image if the category deletion was successful
          if (rowsAffected > 0)
          {
            DeleteImage(subCategoryToDelete);
          }

          return rowsAffected > 0;
        }
        catch (Exception ex)
        {
          // Handling any potential exceptions
          Console.WriteLine($"Error deleting product category: {ex.Message}");
          return false;
        }
      }
    }

    private string GetSubCategoryToDelete(MySqlConnection connection, int categoryId)
    {
      string subCategory = string.Empty;
      MySqlCommand command = new MySqlCommand
      {
        Connection = connection
      };

      string query = "SELECT SubCategory FROM productcategories WHERE CategoryId = @CategoryId";
      command.CommandText = query;
      command.Parameters.AddWithValue("@CategoryId", categoryId);

      // Open the database connection before executing the SQL command
      connection.Open();

      using (MySqlDataReader reader = command.ExecuteReader())
      {
        if (reader.Read())
        {
          subCategory = reader["SubCategory"].ToString();
        }
      }

      // Close the database connection after retrieving the subCategory
      connection.Close();

      return subCategory;
    }




    public ProductCategory GetProductCategory(int id)
    {
      var productCategory = new ProductCategory();
      using (MySqlConnection connection = new(dbConnection))
      {
        MySqlCommand command = new()
        {
          Connection = connection,
        };

        string query = "select * from productCategories where CategoryId =" + id + ";";
        command.CommandText = query;

        connection.Open();

        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
          productCategory.Id = (int)reader["CategoryId"];
          productCategory.Category = (string)reader["Category"];
          productCategory.SubCategory = (string)reader["SubCategory"];
        }
      }
      return productCategory;
    }

    public async Task<byte[]> GetBannerImage(string name)
    {
      // List of supported image file extensions in order of preference.
      List<string> supportedExtensions = new List<string> { ".png", ".jpeg", ".jpg", ".gif" };

      foreach (string extension in supportedExtensions)
      {
        string imagePath = Path.Combine("Resources", "Banner", name + extension);

        // Check if the image file exists with the current extension.
        if (File.Exists(imagePath))
        {
          try
          {
            byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return imageBytes;
          }
          catch (Exception ex)
          {
            // Handling any potential exceptions
            Console.WriteLine($"Error reading image file: {ex.Message}");
            break; // Stop processing if an error occurs.
          }
        }
      }

      // If no image file with supported extensions is found, log the message and return null.
      Console.WriteLine($"Image file not found with any supported extension for: {name}");
      return null;
    }

  }
}
