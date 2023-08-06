using ecommerce.Data.Interfaces;
using ecommerce.Models;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ecommerce.Data
{
  public class UserDataAccess : IUserDataAccess
  {
    private readonly IConfiguration configuration;
    private readonly string dbConnection;
    public UserDataAccess(IConfiguration configuration)
    {
      this.configuration = configuration;
      dbConnection = this.configuration.GetConnectionString("DB");
    }

    public User GetUser(int id)
    {
      var user = new User();
      using (MySqlConnection connection = new(dbConnection))
      {
        MySqlCommand command = new()
        {
          Connection = connection
        };

        string query = "SELECT * FROM Users WHERE UserId = @id;";
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        connection.Open();
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
          user.Id = (int)reader["UserId"];
          user.FirstName = (string)reader["FirstName"];
          user.LastName = (string)reader["LastName"];
          user.Email = (string)reader["Email"];
          user.Address = (string)reader["Address"];
          user.Mobile = (string)reader["Mobile"];
          user.Password = (string)reader["Password"];
          user.CreatedAt = (string)reader["CreatedAt"];
          user.ModifiedAt = (string)reader["ModifiedAt"];
        }
      }
      return user;
    }

    public async Task<bool> InsertUser(User user)
    {
      using (MySqlConnection connection = new(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };
        await connection.OpenAsync();

        string query = "SELECT COUNT(*) FROM Users WHERE Email=@em;";
        command.CommandText = query;
        command.Parameters.Add("@em", MySqlDbType.VarChar).Value = user.Email;
        int count = Convert.ToInt32(command.ExecuteScalar());
        if (count > 0)
        {
          connection.Close();
          return false;
        }

        query = "INSERT INTO Users (FirstName, LastName, Address, Mobile, Email, Password, CreatedAt, ModifiedAt) VALUES (@fn, @ln, @add, @mb, @em, @pwd, @cat, @mat);";

        command.CommandText = query;
        command.Parameters.Clear();
        command.Parameters.Add("@fn", MySqlDbType.VarChar).Value = user.FirstName;
        command.Parameters.Add("@ln", MySqlDbType.VarChar).Value = user.LastName;
        command.Parameters.Add("@add", MySqlDbType.VarChar).Value = user.Address;
        command.Parameters.Add("@mb", MySqlDbType.VarChar).Value = user.Mobile;
        command.Parameters.Add("@em", MySqlDbType.VarChar).Value = user.Email;
        command.Parameters.Add("@pwd", MySqlDbType.VarChar).Value = user.Password;
        command.Parameters.Add("@cat", MySqlDbType.VarChar).Value = user.CreatedAt;
        command.Parameters.Add("@mat", MySqlDbType.VarChar).Value = user.ModifiedAt;

        command.ExecuteNonQuery();
      }
      return true;
    }

    public async Task<List<User>> GetAllUsers()
    {
      var users = new List<User>();

      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection,
        };

        string query = "SELECT * FROM Users WHERE role = 'employee';";

        command.CommandText = query;

        await connection.OpenAsync();

        using (DbDataReader reader = await command.ExecuteReaderAsync())
        {
          while (await reader.ReadAsync())
          {
            var user = new User()
            {
              Id = Convert.ToInt32(reader["UserId"]),
              FirstName = reader["FirstName"].ToString(),
              LastName = reader["LastName"].ToString(),
              Email = reader["Email"].ToString(),
              Address = reader["Address"].ToString(),
              Mobile = reader["Mobile"].ToString(),
              Role = reader["Role"].ToString()
            };

            users.Add(user);
          }
        }
      }

      return users;
    }


    public async Task<string> IsUserPresent(string email, string password)
    {
      User user = new User();
      using (MySqlConnection connection = new(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };

        await connection.OpenAsync();
        string query = "SELECT COUNT(*) FROM Users WHERE Email=@Email AND Password=@Password;";
        command.CommandText = query;
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@Password", password);
        int count = Convert.ToInt32(command.ExecuteScalar());
        if (count == 0)
        {
          connection.Close();
          return "";
        }

        query = "SELECT * FROM Users WHERE Email=@Email AND Password=@Password;";
        command.CommandText = query;

        DbDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
          user.Id = Convert.ToInt32(reader["UserId"]);
          user.FirstName = reader["FirstName"].ToString();
          user.LastName = reader["LastName"].ToString();
          user.Email = reader["Email"].ToString();
          user.Address = reader["Address"].ToString();
          user.Mobile = reader["Mobile"].ToString();
          user.Password = reader["Password"].ToString();
          user.Role = reader["Role"].ToString();
          user.CreatedAt = reader["CreatedAt"].ToString();
          user.ModifiedAt = reader["ModifiedAt"].ToString();
        }

        string key = "b9d786d25e4b3a532d6e214c6c8a2bcf";
        string duration = "60";
        var symmetrickey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(symmetrickey, SecurityAlgorithms.HmacSha256);

        // the data in the tokens will be in claims
        var claims = new[]
        {
            new Claim("id", user.Id.ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName),
            new Claim("address", user.Address),
            new Claim("mobile", user.Mobile),
            new Claim("email", user.Email),
            new Claim("role", user.Role),
            new Claim("createdAt", user.CreatedAt),
            new Claim("modifiedAt", user.ModifiedAt)
        };

        var jwtToken = new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(duration)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
      }
      return "";
    }


  }
}
