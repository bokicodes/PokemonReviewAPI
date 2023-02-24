namespace PokemonReviewAPI.Models;

public class PokemonCategory
{
    public int PokemonId { get; set; } //pamtis da ima nacin na koji pises i on sam zna sta je, npr Id automatski zna da je primarni kljuc
    public int CategoryId { get; set; } // a da si pisao npr GlavniId morao bi iznad [Key] recimo da bi on kad migrira znao, tako i ovo PokemonId je ono praksa na osnovu koje on zna sta je
    public Pokemon Pokemon { get; set; }
    public Category Category { get; set; }
}
