using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Interfaces;

public interface ICountryRepository
{
    ICollection<Country> GetCountries();
    Country GetCountry(int id);
    Country GetCountryByOwnerId(int ownerId);
    ICollection<Owner> GetOwnersByCountryId(int countryId); //namerno da vidis implementaciju ali u controlleru necemo da je ubacimo
    bool CountryExists(int id); // ovo je uvek dobro imati
    bool CreateCountry(Country country);
    bool UpdateCountry(Country country);
    bool DeleteCountry(Country country);
    bool Save();
}
