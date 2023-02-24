using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewAPI.Dto;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;
using PokemonReviewAPI.Repository;

namespace PokemonReviewAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OwnerController : ControllerBase
{
	private readonly IOwnerRepository _ownerRepository;
	private readonly ICountryRepository _countryRepository;
	private readonly IMapper _mapper;

	public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
	{
		_ownerRepository = ownerRepository;
		_countryRepository = countryRepository;
		_mapper = mapper;
	}

	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
	[ProducesResponseType(400)]
	public IActionResult GetOwners()
	{
		var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		return Ok(owners);
	}

    [HttpGet("{ownerId}")]
    [ProducesResponseType(200, Type = typeof(Owner))] // NE MORA OVO, samo cini da API endpoint izgleda malo cleaner, nema neku drugu svrhu
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetOwner(int ownerId)
	{
		if (!_ownerRepository.OwnerExists(ownerId)) // eto cemu sluzi ta metoda :D, mogli smo i bez nje pa da ovde proverimo ali ovako je lepse
			return NotFound();

        var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(owner);
    }

	[HttpGet("pokemon/{ownerId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))] // NE MORA OVO, samo cini da API endpoint izgleda malo cleaner, nema neku drugu svrhu
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
	public IActionResult GetPokemonByOwner(int ownerId)
	{
		if (!_ownerRepository.OwnerExists(ownerId))
			return NotFound();

		var pokemons = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		return Ok(pokemons);
	}


	//POST
	[HttpPost]
	[ProducesResponseType(200)]
	[ProducesResponseType(422)]
	[ProducesResponseType(400)]
	public IActionResult CreateOwner([FromQuery] int countryId,[FromBody] OwnerDto newOwnerDto)
	{
		if (newOwnerDto is null)
			return BadRequest(ModelState);
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var owner = _ownerRepository.GetOwners()
			.Where(o => o.FirstName.Trim().ToUpper() == newOwnerDto.FirstName.Trim().ToUpper()
			&& o.LastName.Trim().ToUpper() == newOwnerDto.LastName.Trim().ToUpper())
			.FirstOrDefault();

		if(owner is not null)
		{
			ModelState.AddModelError("", "Owner already exists");
			return StatusCode(422, ModelState);
		}

		var newOwner = _mapper.Map<Owner>(newOwnerDto);

		// Kada imas ONE relationship onda moras da ubacis i TAJ podatak da bi bilo dobro i da ne bi vracalo error!
		newOwner.Country = _countryRepository.GetCountry(countryId);

		if (!_ownerRepository.CreateOwner(newOwner))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}

		return Ok("Successfuly created");
		
	}


	// PUT (Update)
	[HttpPut("{ownerId}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updateOwnerDto)
	{
		if (updateOwnerDto == null)
			return BadRequest(ModelState);

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		if (ownerId != updateOwnerDto.Id)
			return BadRequest(ModelState);

		if (!_ownerRepository.OwnerExists(ownerId))
			return NotFound();

		//Moja logika je da mora country novog updejtovanog objekta da bude isti
		//kao i proslom objektu. Samo treba da setujemo to

		Owner updateOwner = _mapper.Map<Owner>(updateOwnerDto);

		if (!_ownerRepository.UpdateOwner(updateOwner))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}

		return NoContent();
	}

	//Delete
	[HttpDelete("{ownerId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
	public IActionResult DeleteOwner(int ownerId)
	{
		if (!_ownerRepository.OwnerExists(ownerId))
			return NotFound();

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var ownerDelete = _ownerRepository.GetOwner(ownerId);

		if (!_ownerRepository.DeleteOwner(ownerDelete))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}

		return NoContent();
	}

}
