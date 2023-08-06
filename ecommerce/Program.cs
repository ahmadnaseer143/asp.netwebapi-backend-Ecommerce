using ecommerce.Data;
using ecommerce.Data.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// add cors
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(builder =>
  {
    builder.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
  });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
{
  x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = "localhost",
    ValidAudience = "localhost",
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("verysecret")),
    ClockSkew = TimeSpan.Zero
  };
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add dependecy of DataAccess classes
builder.Services.AddSingleton<IDataAccess, DataAccess>();
builder.Services.AddSingleton<ICategoryDataAccess, CategoryDataAccess>();
builder.Services.AddSingleton<IOfferDataAccess, OfferDataAccess>();
builder.Services.AddSingleton<IUserDataAccess, UserDataAccess>();
builder.Services.AddSingleton<ICartDataAccess, CartDataAccess>();
builder.Services.AddSingleton<IProductDataAccess, ProductDataAccess>();
builder.Services.AddSingleton<IReviewDataAccess, ReviewDataAccess>();
builder.Services.AddSingleton<IPaymentDataAccess, PaymentDataAccess>();
builder.Services.AddSingleton<IOrderDataAccess, OrderDataAccess>();


var app = builder.Build();

app.UseCors();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
