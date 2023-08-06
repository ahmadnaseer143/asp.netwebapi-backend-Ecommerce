using ecommerce.Data.Interfaces;
using ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ecommerce.Data
{
  public class DataAccess : IDataAccess
  {
    private readonly IConfiguration configuration;
    private readonly string dbConnection;
    private readonly string dateformat;
    public DataAccess(IConfiguration configuration)
    {
      this.configuration = configuration;
      dbConnection = this.configuration.GetConnectionString("DB");
      dateformat = this.configuration["Constants:DateFormat"];
    }
  }
}
