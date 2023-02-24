namespace PokemonReviewAPI.Models;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Owner> Owners { get; set; } //u jednoj drzavi mozes da imas vise ownera
}
