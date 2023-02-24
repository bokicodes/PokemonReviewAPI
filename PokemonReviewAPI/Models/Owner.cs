namespace PokemonReviewAPI.Models;

public class Owner
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gym { get; set; }
    public Country Country { get; set; } // one, jedan objekat, owner je iz jedne drzave
    public ICollection<PokemonOwner> PokemonOwners { get; set; }
}
