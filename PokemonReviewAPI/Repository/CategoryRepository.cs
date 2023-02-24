using PokemonReviewAPI.Data;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly DataContext _context;

    public CategoryRepository(DataContext context)
    {
        _context = context;
    }

    public bool CategoryExists(int id)
    {
        return _context.Categories.Any(c => c.Id == id);
    }

    public ICollection<Category> GetCategories()
    {
        var categories = _context.Categories.ToList(); //moze i OrderBy ali posto imamo samo 3 kategorije ne mora da se orderuje realno
        
        return categories;
    }

    public Category GetCategory(int id)
    {
        var category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();

        return category; //tek u Controlleru proveravamo zapravo da li postoji ili ne itd!
                        // ovde samo Db calls
    }

    public ICollection<Pokemon> GetPokemonsByCategoryId(int categoryId)
    {
        var pokemons = _context.PokemonCategories
            .Where(pc => pc.CategoryId == categoryId) //vraca PokemonCategories a meni treba Pokemons
            .Select(p => p.Pokemon) //od svih tih pokemon kategorija sa odgovarajucim ID-em, zelim samo njihove pokemone (moze i Include)
            .ToList();

        return pokemons;
    }

    public bool CreateCategory(Category category)
    {
        //Change tracker -> adding, updating, modifying
        // -> conntected vs disconnected (googlaj ovo)
        // EntityState.Added = je disconnected ali u 99% slucajeva radimo connected state. Googlaj
        // Kada nismo u DB Contextu to znaci da je disconnected entity state
        _context.Add(category); //moze i _context.Categories.Add(category), ali posto je objekat tipa Category on zna da se radi o njemu

        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges(); 
        // SaveChanges() je zapravo ono sto posalje novinu/promenu u bazu i sacuva to. Vraca intiger
        // Mislim da vraca broj novih entity-a u bazi ili mozda broj promena, zato gore > 0, ako ima neka
        // promena onda vracamo dole true, ako nema nista onda vracamo dole false i to znaci da se nista nije desilo.

        return saved > 0 ? true : false;
    }

    public bool UpdateCategory(Category category)
    {
        _context.Update(category);
        return Save(); //isto koristi Save();
    }

    public bool DeleteCategory(Category category)
    {
        _context.Remove(category);
        return Save();
    }
}
