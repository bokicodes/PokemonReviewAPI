using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewAPI.Dto;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PokemonController : ControllerBase
{
	private readonly IPokemonRepository _pokemonRepository;
	private readonly IMapper _mapper;
	private readonly IOwnerRepository _ownerRepository;
	private readonly ICategoryRepository _categoryRepository;
	private readonly IReviewRepository _reviewRepository;

	public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper,
		IOwnerRepository ownerRepository, ICategoryRepository categoryRepository,
		IReviewRepository reviewRepository)
	{
		_pokemonRepository = pokemonRepository;
		_mapper = mapper;
		_ownerRepository = ownerRepository;
		_categoryRepository = categoryRepository;
		_reviewRepository = reviewRepository;
	}

	[HttpGet]
	[ProducesResponseType(200,Type = typeof(IEnumerable<Pokemon>))] //moze i ICollection ovde, a moze i IEnumerable jer samo vracamo podatke, nema dodavanja/brisanja itd
	[ProducesResponseType(400)]
	public IActionResult GetPokemons() // obavezno IActionResult a ne ActionResult !!!
	{
		var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

		if (!ModelState.IsValid) //proverava da li je model validan, npr ako se umesto
			return BadRequest(ModelState);	//pokemona posalje ker on ce to da shvati xD

		return Ok(pokemons);
	}

	[HttpGet("{pokeId}")]
	[ProducesResponseType(200, Type = typeof(Pokemon))] // NE MORA OVO, samo cini da API endpoint izgleda malo cleaner, nema neku drugu svrhu
	[ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetPokemon(int pokeId)
	{
		if (!_pokemonRepository.PokemonExists(pokeId)) // eto cemu sluzi ta metoda :D, mogli smo i bez nje pa da ovde proverimo ali ovako je lepse
			return NotFound();

		var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		return Ok(pokemon);
	}

	[HttpGet("rating/{pokeId}")]
    [ProducesResponseType(200, Type = typeof(decimal))] //typeof(tip koji vracamo) 
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetPokemonRating(int pokeId)
	{ // IActionResult je najnoviji najbolji nacin za vracanje ovih status codova i rezulta
		// ti mozes cak i da vratis samo string od API-a, da je gore povratna vrednost string umesto IActionResult
		// to skroz moze... Ali je najbolje ovo da se vraca
		if (!_pokemonRepository.PokemonExists(pokeId))
			return NotFound();

		var rating = _pokemonRepository.GetPokemonRating(pokeId);

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		return Ok(rating);
	}

	// POST(CREATE)
	//Kada god pravis pokemona on mora da ima ownera i mora da ima kategoriju, zato ubacujes to isto!
	[HttpPost]
	[ProducesResponseType(200)] // ipak ovo treba jer vracamo No Content izgleda
	[ProducesResponseType(422)]
	[ProducesResponseType(400)]
	public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto newPokemonDto)
	{ 
		if (newPokemonDto is null)
			return BadRequest(ModelState);
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		if (!_categoryRepository.CategoryExists(categoryId))
			return NotFound();
		if (!_ownerRepository.OwnerExists(ownerId))
			return NotFound();

		var pokemon = _pokemonRepository.GetPokemons()
			.Where(p => p.Name.Trim().ToUpper() == newPokemonDto.Name.Trim().ToUpper())
			.FirstOrDefault();

		if(pokemon is not null)
		{
			ModelState.AddModelError("", "Pokemon already exists");
			return StatusCode(422, ModelState);
		}
		var newPokemon = _mapper.Map<Pokemon>(newPokemonDto);
		
		// Ne mora kao za One da eksplicitno ubacujemo, nego imamo lepo u Repository metodu za to, samo posaljem parametre i tjt :D
		if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, newPokemon))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}

		return Ok("Successfuly created");
	}



	// PUT (Update)
	[HttpPut("{pokemonId}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	public IActionResult UpdatePokemon(int pokemonId,
		[FromQuery] int ownerId, [FromQuery] int categoryId,
		[FromBody] PokemonDto updatePokemonDto)
	{
		if (updatePokemonDto == null)
			return BadRequest(ModelState);

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		if (pokemonId != updatePokemonDto.Id)
			return BadRequest(ModelState);

		if (!_pokemonRepository.PokemonExists(pokemonId))
			return NotFound();

		if (!_ownerRepository.OwnerExists(ownerId))
			return NotFound();

		if (!_categoryRepository.CategoryExists(categoryId))
			return NotFound();

		var updatePokemon = _mapper.Map<Pokemon>(updatePokemonDto);

		if (!_pokemonRepository.UpdatePokemon(ownerId,categoryId,updatePokemon))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}

		return NoContent();
	}


	// Delete
	[HttpDelete("{pokemonId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
	public IActionResult DeletePokemon(int pokemonId)
	{
		if (!_pokemonRepository.PokemonExists(pokemonId))
			return NotFound();

		if (!ModelState.IsValid)
			return BadRequest();

		var reviewsOfPokemon = _reviewRepository.GetReviewsOfAPokemon(pokemonId);
		//sada vidis cemu nam sluzi ova get metoda, bez nje tesko bismo mogli ovo sada :D
		var pokemonDelete = _pokemonRepository.GetPokemon(pokemonId);

		if (!_reviewRepository.DeleteReviews(reviewsOfPokemon.ToList())) //uvek dodas ovo ToList da EF tacno zna
        {
			ModelState.AddModelError("", "Something went wrong while deleting reviews of a pokemon.");
			return StatusCode(500, ModelState);
		}

		if (!_pokemonRepository.DeletePokemon(pokemonDelete))
		{
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

		return NoContent();
	}
}
