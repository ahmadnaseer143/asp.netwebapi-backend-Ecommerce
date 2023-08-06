using ecommerce.Models;

namespace ecommerce.Data.Interfaces
{
  public interface IOfferDataAccess
  {

    Offer GetOffer(int id);

    Task<List<Offer>> GetAllOffers();
    Task<bool> InsertOffer(Offer offer);
  }
}
