using ecommerce.Data.Interfaces;
using ecommerce.Models;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace ecommerce.Data
{
  public class CartDataAccess : ICartDataAccess
  {
    private readonly IConfiguration configuration;
    private readonly string dbConnection;
    private readonly IUserDataAccess iUserDataAccess;
    private readonly IProductDataAccess iProductDataAccess;
    public CartDataAccess(IConfiguration configuration, IUserDataAccess iUserDataAccess, IProductDataAccess iProductDataAccess)
    {
      this.configuration = configuration;
      dbConnection = this.configuration.GetConnectionString("DB");
      this.iUserDataAccess = iUserDataAccess;
      this.iProductDataAccess = iProductDataAccess;
    }

    public async Task<bool> InsertCartItem(int userId, int productId)
    {
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };

        await connection.OpenAsync();
        string query = "SELECT COUNT(*) FROM Carts WHERE UserId=" + userId + " AND Ordered='false';";
        command.CommandText = query;
        int count = Convert.ToInt32(command.ExecuteScalar());
        if (count == 0)
        {
          query = "INSERT INTO Carts (UserId, Ordered, OrderedOn) VALUES (" + userId + ", 'false', '');";
          command.CommandText = query;
          command.ExecuteNonQuery();
        }

        query = "SELECT CartId FROM Carts WHERE UserId=" + userId + " AND Ordered='false';";
        command.CommandText = query;
        int cartId = Convert.ToInt32(command.ExecuteScalar());


        query = "INSERT INTO CartItems (CartId, ProductId) VALUES (" + cartId + ", " + productId + ");";
        command.CommandText = query;
        command.ExecuteNonQuery();
        return true;
      }
    }

    public async Task<bool> RemoveCartItem(int userId, int productId)
    {
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };

        await connection.OpenAsync();

        string query = "SELECT CartId FROM Carts WHERE UserId = " + userId + " AND Ordered = 'false';";
        command.CommandText = query;
        int cartId = Convert.ToInt32(command.ExecuteScalar());

        if (cartId != 0)
        {
          query = "DELETE FROM CartItems WHERE CartId = " + cartId + " AND ProductId = " + productId + ";";
          command.CommandText = query;
          int rowsAffected = command.ExecuteNonQuery();

          if (rowsAffected > 0)
          {
            // Item successfully removed from the cart
            return true;
          }
        }

        // Either the cart or the item was not found
        return false;
      }
    }


    public async Task<Cart> GetActiveCartOfUser(int userid)
    {
      var cart = new Cart();
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };
        await connection.OpenAsync();

        string query = "SELECT COUNT(*) From Carts WHERE UserId=" + userid + " AND Ordered='false';";
        command.CommandText = query;

        int count = Convert.ToInt32(command.ExecuteScalar());
        if (count == 0)
        {
          return cart;
        }

        query = "SELECT CartId From Carts WHERE UserId=" + userid + " AND Ordered='false';";
        command.CommandText = query;

        int cartid = Convert.ToInt32(command.ExecuteScalar());

        query = "SELECT * FROM CartItems WHERE CartId=" + cartid + ";";
        command.CommandText = query;

        using (MySqlDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            CartItem item = new CartItem()
            {
              Id = (int)reader["CartItemId"],
              Product = iProductDataAccess.GetProduct((int)reader["ProductId"])
            };
            cart.CartItems.Add(item);
          }
        }

        cart.Id = cartid;
        cart.User = iUserDataAccess.GetUser(userid);
        cart.Ordered = false;
        cart.OrderedOn = "";
      }
      return cart;
    }

    async Task<List<Cart>> ICartDataAccess.GetAllPreviousCartsOfUser(int userid)
    {
      var carts = new List<Cart>();
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };
        string query = "SELECT CartId FROM Carts WHERE UserId=" + userid + " AND Ordered='true';";
        command.CommandText = query;
        await connection.OpenAsync();
        using (MySqlDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            var cartid = (int)reader["CartId"];
            carts.Add(GetCart(cartid));
          }
        }
      }
      return carts;
    }

    public Cart GetCart(int cartid)
    {
      var cart = new Cart();
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };
        connection.Open();

        string query = "SELECT * FROM CartItems WHERE CartId=" + cartid + ";";
        command.CommandText = query;

        using (MySqlDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            CartItem item = new CartItem()
            {
              Id = (int)reader["CartItemId"],
              Product = iProductDataAccess.GetProduct((int)reader["ProductId"])
            };
            cart.CartItems.Add(item);
          }
        }

        query = "SELECT * FROM Carts WHERE CartId=" + cartid + ";";
        command.CommandText = query;

        using (MySqlDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            cart.Id = cartid;
            cart.User = iUserDataAccess.GetUser((int)reader["UserId"]);
            cart.Ordered = bool.Parse((string)reader["Ordered"]);
            cart.OrderedOn = (string)reader["OrderedOn"];
          }
        }
      }
      return cart;
    }

    /*

    async Task<Cart> IDataAccess.GetActiveCartOfUser(int userid)
    {
      var cart = new Cart();
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };
        await connection.OpenAsync();

        string query = "SELECT COUNT(*) From Carts WHERE UserId=" + userid + " AND Ordered='false';";
        command.CommandText = query;

        int count = Convert.ToInt32(command.ExecuteScalar());
        if (count == 0)
        {
          return cart;
        }

        query = "SELECT CartId From Carts WHERE UserId=" + userid + " AND Ordered='false';";
        command.CommandText = query;

        int cartid = Convert.ToInt32(command.ExecuteScalar());

        query = "SELECT * FROM CartItems WHERE CartId=" + cartid + ";";
        command.CommandText = query;

        using (MySqlDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            CartItem item = new CartItem()
            {
              Id = (int)reader["CartItemId"],
              Product = GetProduct((int)reader["ProductId"])
            };
            cart.CartItems.Add(item);
          }
        }

        cart.Id = cartid;
        cart.User = iUserDataAccess.GetUser(userid);
        cart.Ordered = false;
        cart.OrderedOn = "";
      }
      return cart;
    }

    async Task<List<Cart>> IDataAccess.GetAllPreviousCartsOfUser(int userid)
    {
      var carts = new List<Cart>();
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };
        string query = "SELECT CartId FROM Carts WHERE UserId=" + userid + " AND Ordered='true';";
        command.CommandText = query;
        await connection.OpenAsync();
        using (MySqlDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            var cartid = (int)reader["CartId"];
            carts.Add(GetCart(cartid));
          }
        }
      }
      return carts;
    }

     */
  }
}
