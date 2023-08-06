using ecommerce.Data.Interfaces;
using ecommerce.Models;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace ecommerce.Data
{
  public class ProductDataAccess : IProductDataAccess
  {
    private readonly IConfiguration configuration;
    private readonly string dbConnection;
    private readonly ICategoryDataAccess iCategoryDataAccess;
    private readonly IOfferDataAccess iOfferDataAccess;
    private readonly IWebHostEnvironment _hostEnvironment;
    public ProductDataAccess(IConfiguration configuration, IWebHostEnvironment hostEnvironment, ICategoryDataAccess iCategoryDataAccess, IOfferDataAccess iOfferDataAccess)
    {
      this.configuration = configuration;
      dbConnection = this.configuration.GetConnectionString("DB");
      _hostEnvironment = hostEnvironment;
      this.iCategoryDataAccess = iCategoryDataAccess;
      this.iOfferDataAccess = iOfferDataAccess;
    }

    public async Task<List<Product>> GetProducts(string category, string subCategory, int count)
    {
      var products = new List<Product>();
      using (MySqlConnection connection = new(dbConnection))
      {
        MySqlCommand command = new()
        {
          Connection = connection,
        };

        string query = "SELECT * FROM Products WHERE CategoryId = (SELECT CategoryId FROM ProductCategories WHERE Category = @c AND SubCategory = @s) ORDER BY RAND() LIMIT " + count + ";";


        command.CommandText = query;

        command.Parameters.AddWithValue("@c", category);
        command.Parameters.AddWithValue("@s", subCategory);

        await connection.OpenAsync();

        DbDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
          var product = new Product()
          {
            Id = (int)reader["ProductId"],
            Title = (string)reader["Title"],
            Description = (string)reader["Description"],
            Price = Convert.ToDouble(reader["Price"]),
            Quantity = (int)reader["Quantity"],
            ImageName = (string)reader["ImageName"],
          };

          var categoryId = (int)reader["CategoryId"];
          product.ProductCategory = iCategoryDataAccess.GetProductCategory(categoryId);

          var offerId = (int)reader["OfferId"];
          product.Offer = iOfferDataAccess.GetOffer(offerId);

          products.Add(product);
        }
      }
      return products;
    }

    public async Task<byte[]> GetProductImage(int productId)
    {
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection,
          CommandText = "SELECT ImageName FROM Products WHERE ProductId = @productId"
        };

        command.Parameters.AddWithValue("@productId", productId);

        await connection.OpenAsync();

        object result = await command.ExecuteScalarAsync();
        if (result != null && result != DBNull.Value)
        {
          string imagePath = result.ToString();
          byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
          return imageBytes;
        }
      }

      return null;
    }

    public async Task<List<Product>> GetAllProducts()
    {
      var products = new List<Product>();
      using (MySqlConnection connection = new(dbConnection))
      {
        MySqlCommand command = new()
        {
          Connection = connection,
        };

        string query = "SELECT * FROM Products;";


        command.CommandText = query;

        await connection.OpenAsync();

        DbDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
          var product = new Product()
          {
            Id = (int)reader["ProductId"],
            Title = (string)reader["Title"],
            Description = (string)reader["Description"],
            Price = Convert.ToDouble(reader["Price"]),
            Quantity = (int)reader["Quantity"],
            ImageName = (string)reader["ImageName"],

          };

          var categoryId = (int)reader["CategoryId"];
          product.ProductCategory = iCategoryDataAccess.GetProductCategory(categoryId);

          var offerId = (int)reader["OfferId"];
          product.Offer = iOfferDataAccess.GetOffer(offerId);

          products.Add(product);
        }
      }
      return products;
    }

    public Product GetProduct(int id)
    {
      var product = new Product();
      using (MySqlConnection connection = new(dbConnection))
      {
        MySqlCommand command = new()
        {
          Connection = connection,
        };

        string query = "select * from products where ProductId =" + id + ";";
        command.CommandText = query;

        connection.Open();

        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
          product.Id = (int)reader["ProductId"];
          product.Title = (string)reader["Title"];
          product.Description = (string)reader["Description"];
          product.Price = Convert.ToDouble(reader["Price"]);
          product.Quantity = (int)reader["Quantity"];
          product.ImageName = (string)reader["ImageName"];

          // Get the image file as base64 string
          if (!string.IsNullOrEmpty(product.ImageName))
          {
            byte[] fileBytes = System.IO.File.ReadAllBytes(product.ImageName);
            string base64String = Convert.ToBase64String(fileBytes);
            product.ImageFile = base64String;
          }

          var categoryId = (int)reader["CategoryId"];
          product.ProductCategory = iCategoryDataAccess.GetProductCategory(categoryId);

          var offerId = (int)reader["OfferId"];
          product.Offer = iOfferDataAccess.GetOffer(offerId);
        }
      }
      return product;
    }

    public async Task<Product> UpdateProduct(Product product)
    {
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection,
        };

        string query = "UPDATE products SET Title = @Title, Description = @Description, Price = @Price, Quantity = @Quantity, ImageName = @ImageName, CategoryId = @CategoryId, OfferId = @OfferId WHERE ProductId = @ProductId;";
        command.CommandText = query;

        // Set the parameter values
        command.Parameters.AddWithValue("@Title", product.Title);
        command.Parameters.AddWithValue("@Description", product.Description);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@Quantity", product.Quantity);
        command.Parameters.AddWithValue("@ImageName", product.ImageName);
        command.Parameters.AddWithValue("@CategoryId", product.ProductCategory.Id);
        command.Parameters.AddWithValue("@OfferId", product.Offer.Id);
        command.Parameters.AddWithValue("@ProductId", product.Id);

        await connection.OpenAsync();

        int rowsAffected = command.ExecuteNonQuery();

        // Check if any rows were affected by the update
        if (rowsAffected > 0)
        {
          // Check if there is an updated image file
          if (!string.IsNullOrEmpty(product.ImageFile))
          {
            // Delete the previous image file
            if (!string.IsNullOrEmpty(product.ImageName))
            {
              File.Delete(product.ImageName);
            }

            // Convert the Base64 string to a byte array
            byte[] fileBytes = Convert.FromBase64String(product.ImageFile);

            string fileName = $"{product.Id}.jpg";

            // Define the base directory path and create it if it doesn't exist
            var baseDirectory = Path.Combine(_hostEnvironment.WebRootPath ?? string.Empty, "Resources", "Images");
            Directory.CreateDirectory(baseDirectory);

            // Get the category and subcategory folder paths
            var categoryFolder = Path.Combine(baseDirectory, product.ProductCategory.Category);
            var subcategoryFolder = Path.Combine(categoryFolder, product.ProductCategory.SubCategory);

            // Create category and subcategory folders if they don't exist
            Directory.CreateDirectory(categoryFolder);
            Directory.CreateDirectory(subcategoryFolder);

            // Define the folder path for the productId
            string productIdFolder = Path.Combine(subcategoryFolder, product.Id.ToString());
            Directory.CreateDirectory(productIdFolder);

            // Define the file path
            string filePath = Path.Combine(productIdFolder, fileName);

            // Save the byte array to a file
            await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

            // Update the product's ImageName attribute with the file path
            product.ImageName = filePath;
          }

          // Update the image path in the database
          command.CommandText = "UPDATE products SET ImageName = @ImageName WHERE ProductId = @ProductId;";
          command.Parameters.Clear();
          command.Parameters.AddWithValue("@ImageName", product.ImageName);
          command.Parameters.AddWithValue("@ProductId", product.Id);
          command.ExecuteNonQuery();
          return product;
        }
      }

      // If no rows were affected or an error occurred, return null
      return null;
    }

    public async Task<int> InsertProduct(Product product)
    {
      var productCategory = new ProductCategory();
      productCategory = iCategoryDataAccess.GetProductCategory(product.ProductCategory.Id);

      if (productCategory == null)
      {
        return -1;
      }

      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };
        await connection.OpenAsync();

        string query = "INSERT INTO Products (Title, Description, CategoryId, OfferId, Price, Quantity, ImageName) " +
                       "VALUES (@title, @description, @categoryId, @offerId, @price, @quantity, @imageName);";

        command.CommandText = query;
        command.Parameters.AddWithValue("@title", product.Title);
        command.Parameters.AddWithValue("@description", product.Description);
        command.Parameters.AddWithValue("@categoryId", product.ProductCategory.Id);
        command.Parameters.AddWithValue("@offerId", product.Offer.Id);
        command.Parameters.AddWithValue("@price", product.Price);
        command.Parameters.AddWithValue("@quantity", product.Quantity);
        command.Parameters.AddWithValue("@imageName", product.ImageName);

        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
          command.CommandText = "SELECT LAST_INSERT_ID();";
          int productId = Convert.ToInt32(command.ExecuteScalar());

          //upload image from imageFile
          if (!string.IsNullOrEmpty(product.ImageFile))
          {
            // Convert the Base64 string to a byte array
            byte[] fileBytes = Convert.FromBase64String(product.ImageFile);

            // Generate a unique filename using the current timestamp
            string timestamp = DateTime.Now.Ticks.ToString();
            string fileName = $"{productId}.jpg";

            // Define the base directory path and create it if it doesn't exist
            var baseDirectory = Path.Combine(_hostEnvironment.WebRootPath ?? string.Empty, "Resources", "Images");
            Directory.CreateDirectory(baseDirectory);

            // Get the category and subcategory folder paths
            var categoryFolder = Path.Combine(baseDirectory, productCategory.Category);
            var subcategoryFolder = Path.Combine(categoryFolder, productCategory.SubCategory);

            // Create category and subcategory folders if they don't exist
            Directory.CreateDirectory(categoryFolder);
            Directory.CreateDirectory(subcategoryFolder);

            // Define the folder path for the productId
            string productIdFolder = Path.Combine(subcategoryFolder, productId.ToString());
            Directory.CreateDirectory(productIdFolder);

            // Define the file path
            string filePath = Path.Combine(productIdFolder, fileName);

            // Save the byte array to a file
            await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

            // Update the product's ImageName attribute with the file path
            product.ImageName = filePath;
          }

          // save the image path to imageName in mysql
          command.CommandText = "UPDATE Products SET ImageName = @imageName WHERE ProductId = @productId";
          command.Parameters.Clear();
          command.Parameters.AddWithValue("@imageName", product.ImageName);
          command.Parameters.AddWithValue("@productId", productId);
          command.ExecuteNonQuery();
          return productId;
        }
        else
        {
          return -1;
        }
      }
    }

    public async Task<bool> DeleteProduct(int id, string category, string subCategory)
    {
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };
        await connection.OpenAsync();

        string query = "DELETE FROM Products WHERE ProductId = @id;";
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
          // Define the base directory path
          var baseDirectory = Path.Combine(_hostEnvironment.WebRootPath ?? string.Empty, "Resources", "Images");

          // Get the category and subcategory folder paths
          var categoryFolder = Path.Combine(baseDirectory, category);
          var subcategoryFolder = Path.Combine(categoryFolder, subCategory);

          // Define the folder path for the productId
          string productIdFolder = Path.Combine(subcategoryFolder, id.ToString());

          // Delete the folder if it exists
          if (Directory.Exists(productIdFolder))
          {
            Directory.Delete(productIdFolder, true);
          }

          return true;
        }
        return false;
      }
    }


  }
}
