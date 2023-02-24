namespace PokemonReviewAPI.Dto;

public class PokemonDto
{ // Ne zelis navigation properties da ti se prikazuju, samo glavni podaci za model!
    public int Id { get; set; } 
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
}
