using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Interfaces;

public interface IOwnerRepository
{
    ICollection<Owner> GetOwners();    
    Owner GetOwner(int ownerId);
    ICollection<Owner> GetOwnerOfAPokemon(int pokeId); //samo radi implementacije, nece da pravimo http endpoint za to, kaze u praksi tesko da bi nekom trebalo ovo
                                                            //ako ces da stavljas na github msm da treba ove metode koje nisu za endpoint da prebacis u vezbu i obrises odavde
    ICollection<Pokemon> GetPokemonByOwner(int ownerId); 
    bool OwnerExists(int ownerId);
    bool CreateOwner(Owner owner);
    bool UpdateOwner(Owner owner);
    bool DeleteOwner(Owner owner);
    bool Save();
}
