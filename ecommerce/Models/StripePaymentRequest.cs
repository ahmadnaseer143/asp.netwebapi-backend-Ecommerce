namespace ecommerce.Models
{
  public class StripePaymentRequest
  {
    public string StripeToken { get; set; }
    public int Amount { get; set; }
    public string Description { get; set; }

    public string CustomerName { get; set; }
    public string CustomerAddress { get; set; }
  }
}

