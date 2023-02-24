using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Interfaces;

public interface IPokemonRepository
{
    ICollection<Pokemon> GetPokemons(); //metoda koju nas PokemonRepository treba da implementira
    Pokemon GetPokemon(int id);
    Pokemon GetPokemon(string name);
    decimal GetPokemonRating(int id);
    bool PokemonExists(int id);
    bool CreatePokemon(int ownerId,int categoryId, Pokemon pokemon); // mora jer imamo many to many relationships!
    bool UpdatePokemon(int ownerId,int categoryId, Pokemon pokemon);
    bool DeletePokemon(Pokemon pokemon);
    bool Save();
}
