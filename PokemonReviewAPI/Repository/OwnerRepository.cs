using PokemonReviewAPI.Data;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Repository;

public class OwnerRepository : IOwnerRepository
{
    private readonly DataContext _context;

    public OwnerRepository(DataContext context) //googlaj constructor injection da naucis u teroiji to
    {
        _context = context;
    }

    public Owner GetOwner(int ownerId)
    {
        return _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
    }

    public ICollection<Owner> GetOwnerOfAPokemon(int pokeId)
    {
        return _context.PokemonOwners.Where(po => po.Pokemon.Id == pokeId)
            .Select(o => o.Owner).ToList();
        //proveri da li bi isto vracalo za po.PokemonId
    }

    public ICollection<Owner> GetOwners()
    {
        return _context.Owners.OrderBy(o => o.Id).ToList();
        //u praksi ovo bi se sredilo malo, order by i jos neko sredjivanje, da znas
    }

    public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
    {
        return _context.PokemonOwners.Where(o => o.Owner.Id == ownerId).Select(p => p.Pokemon).ToList();
    }

    public bool OwnerExists(int ownerId)
    {
        return _context.Owners.Any(o => o.Id == ownerId);
    }

    public bool CreateOwner(Owner owner)
    {
        _context.Add(owner);

        return Save();
    }
    public bool Save()
    {
        return _context.SaveChanges() > 0;
    }

    public bool UpdateOwner(Owner owner)
    {
        _context.Update(owner);
        return Save();
    }

    public bool DeleteOwner(Owner owner)
    {
        _context.Remove(owner);
        return Save();
    }
}
