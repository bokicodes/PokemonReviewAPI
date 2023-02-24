using PokemonReviewAPI.Data;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Repository;

public class CountryRepository : ICountryRepository
{
    private readonly DataContext _context;

    public CountryRepository(DataContext context)
    {
        _context = context;
    }

    public bool CountryExists(int id)
    {
        return _context.Countries.Any(c => c.Id == id);
    }

    public ICollection<Country> GetCountries()
    {
        return _context.Countries.OrderBy(c => c.Id).ToList();
    }

    public Country GetCountry(int id)
    {
        return _context.Countries.Where(c => c.Id == id).FirstOrDefault();
    }

    public Country GetCountryByOwnerId(int ownerId)
    {
        return _context.Owners.Where(o => o.Id == ownerId)
            .Select(c => c.Country).FirstOrDefault();

    }

    public ICollection<Owner> GetOwnersByCountryId(int countryId)
    {
        return _context.Owners.Where(o => o.Country.Id == countryId).ToList();
                                        // moze jer Owners ima property Country!
                                        // Owner je iz jedne Zemlje.
                                        // a to List jer moze da imamo vise Ownera iz jedne Zemlje
                                        // to imas i u Modelima ako pogledas veze bice ti sve jasno
    }

    // POST methods
    public bool CreateCountry(Country country)
    {
        _context.Add(country); // na osnovu Country country zna da dodajemo u tabelu Country! :D

        return Save(); //vraca true ili false
    }
    public bool Save()
    {
        return _context.SaveChanges() > 0; // efikasniji nacin :D
    }

    public bool UpdateCountry(Country country)
    {
        _context.Update(country);
        return Save();
    }

    public bool DeleteCountry(Country country)
    {
        _context.Remove(country);
        return Save();
    }
}
