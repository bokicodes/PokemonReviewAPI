using PokemonReviewAPI.Data;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Repository;

public class PokemonRepository : IPokemonRepository
{
	private readonly DataContext _context;

	public PokemonRepository(DataContext context)
	{
		_context = context;
	}

	public ICollection<Pokemon> GetPokemons()
    {
        return _context.Pokemons.OrderBy(p => p.Id).ToList();
    }

    public Pokemon GetPokemon(int id)
    {
        return _context.Pokemons.Where(p => p.Id == id).FirstOrDefault();
        // .FirstOrDefault() je dobro staviti, da ne bude error, znaci bukvalno prvi na koji naidje
        // i  tako sigurno znamo da je 1 pokemon, jer 1 i treba da vrati
    }

    public Pokemon GetPokemon(string name)
    {
        return _context.Pokemons.Where(p => p.Name.Equals(name)).FirstOrDefault();
    }

    public decimal GetPokemonRating(int id)
        // Ima vise reviews jednog pokemona, svaki review je ostavio svoj rating
    {
        var review = _context.Reviews.Where(r => r.Pokemon.Id == id); //OVO VRACA LISTU!
        // uzimamo listu svih reviewa za tog pokemona koji nam je prosledjen

        if (review.Count() <= 0) //ako nema review-a vraca 0 jednostavno
            return 0;

        //ako ima review-a:
        var sumOfRatings = review.Sum(r => r.Rating); //sabira ratinge svih review-a! (sabira reviewe po ratingu)
        var averageRating = (decimal)sumOfRatings / review.Count(); //i nalazi njegov prosek tako sto deli sa brojem review-a

        return averageRating; //i vraca prosecan rating Pokemona :D
    }

    public bool PokemonExists(int id)
    {
        return _context.Pokemons.Any(p => p.Id == id); //proverava da li se u Pokemons sadrzi neki pokemon sa id == pokeId
            // vraca boolean value naravno
    }

    public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
    {
        var owner = _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
        var category = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();

        // Zapamti da se ovo radi za [HttpPost] kada model ima many to many relationship!
        // Moras i Joined table da popunis, ne moze njih da ostavis prazne jer su one u vezi sa modelom Pokemon u ovom slucaju...
        var pokemonOwner = new PokemonOwner()
        {
            Owner = owner,
            Pokemon = pokemon //u joined tabeli smo povezali naseg novog pokemona sa ovim ownerom!
        }; // a id-eve ce sam da im dodeli, to sam ef radi, ovo moras ti

        _context.Add(pokemonOwner); //dodaje u joined table PokemonOwner

        var pokemonCategory = new PokemonCategory()
        {
            Category = category,
            Pokemon = pokemon //u joined tabeli smo povezali naseg novog pokemona sa ovom kategorijom!
        };

        _context.Add(pokemonCategory);
        //Dakle u sustini ne mozes da iskuliras joined tables jer onda bi malo unistio many to many veze!
        // Mozda cak i moze, ali ne zelis da nikad napravis Pokemona a da mu fale neki podaci kao npr za joined tables!


        //tek sada dodajemo Pokemona, kada smo dodali i ono sto treba u sve joined tabele koje ga se ticu!
        _context.Add(pokemon); //i na kraju dodajemo pokemona, koji ce da ima i odgovarajuceg ownera i category(sto mora zbog many to many veze) i plus sve ostalo sto ovaj pokemon ima sto cemo u [FromBody] da stavimo :D

        return Save();
    }

    public bool Save()
    {
        return _context.SaveChanges() > 0;
    }

    public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
    {
        var owner = _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
        var category = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();

        PokemonOwner pokemonOwner = new PokemonOwner()
        {
            Owner = owner,
            Pokemon = pokemon
        };

        PokemonCategory pokemonCategory = new()
        {
            Category = category,
            Pokemon = pokemon
        };


        _context.Update(pokemonOwner);
        _context.Update(pokemonCategory);

        _context.Update(pokemon);

        return Save();
    }

    public bool DeletePokemon(Pokemon pokemon)
    {
        _context.Remove(pokemon);
        return Save();
    }
}
