namespace PokemonReviewAPI.Models;

public class Pokemon // tabela u bazi
{
    public int Id { get; set; } // kolona u tabeli u bazi
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public ICollection<Review> Reviews { get; set; } //many, lista objekata
    public ICollection<PokemonOwner> PokemonOwners { get; set; }
    public ICollection<PokemonCategory> PokemonCategories { get; set; }
}
