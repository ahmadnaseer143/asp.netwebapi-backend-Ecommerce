using ecommerce.Data.Interfaces;
using ecommerce.Models;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace ecommerce.Data
{
  public class PaymentDataAccess : IPaymentDataAccess
  {
    private readonly IConfiguration configuration;
    private readonly string dbConnection;
    private readonly IUserDataAccess iUserDataAccess;
    private readonly IProductDataAccess iProductDataAccess;
    public PaymentDataAccess(IConfiguration configuration, IUserDataAccess iUserDataAccess, IProductDataAccess iProductDataAccess)
    {
      this.configuration = configuration;
      dbConnection = this.configuration.GetConnectionString("DB");
      this.iUserDataAccess = iUserDataAccess;
      this.iProductDataAccess = iProductDataAccess;
    }

    async Task<List<PaymentMethod>> IPaymentDataAccess.GetPaymentMethods()
    {
      var result = new List<PaymentMethod>();
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };

        string query = "SELECT * FROM PaymentMethods;";
        command.CommandText = query;

        await connection.OpenAsync();

        DbDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
          PaymentMethod paymentMethod = new PaymentMethod()
          {
            Id = (int)reader["PaymentMethodId"],
            Type = (string)reader["Type"],
            Provider = (string)reader["Provider"],
            Available = !Convert.IsDBNull(reader["Available"]) && ((string)reader["Available"]).Equals("Yes", StringComparison.OrdinalIgnoreCase),
            Reason = reader.IsDBNull(reader.GetOrdinal("Reason")) ? null : (string)reader["Reason"]
          };
          result.Add(paymentMethod);
        }
      }
      return result;
    }


    public async Task<int> InsertPayment(Payment payment)
    {
      int value = 0;
      using (MySqlConnection connection = new MySqlConnection(dbConnection))
      {
        MySqlCommand command = new MySqlCommand()
        {
          Connection = connection
        };

        string query = @"INSERT INTO Payments (PaymentMethodId, UserId, TotalAmount, ShippingCharges, AmountReduced, AmountPaid, CreatedAt) 
                        VALUES (@pmid, @uid, @ta, @sc, @ar, @ap, @cat);";

        command.CommandText = query;
        command.Parameters.Add("@pmid", MySqlDbType.Int32).Value = payment.PaymentMethod.Id;
        command.Parameters.Add("@uid", MySqlDbType.Int32).Value = payment.User.Id;
        command.Parameters.Add("@ta", MySqlDbType.VarChar).Value = payment.TotalAmount;
        command.Parameters.Add("@sc", MySqlDbType.VarChar).Value = payment.ShippingCharges;
        command.Parameters.Add("@ar", MySqlDbType.VarChar).Value = payment.AmountReduced;
        command.Parameters.Add("@ap", MySqlDbType.VarChar).Value = payment.AmountPaid;
        command.Parameters.Add("@cat", MySqlDbType.VarChar).Value = payment.CreatedAt;

        await connection.OpenAsync();
        value = command.ExecuteNonQuery();

        if (value > 0)
        {
          query = "SELECT Id FROM Payments ORDER BY Id DESC LIMIT 1;";
          command.CommandText = query;
          value = Convert.ToInt32(command.ExecuteScalar());
        }
        else
        {
          value = 0;
        }
      }
      return value;
    }
  }
}
