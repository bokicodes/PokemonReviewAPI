using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewAPI.Dto;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController : ControllerBase
{
	private readonly ICountryRepository _countryRepository;
	private readonly IMapper _mapper;

	public CountryController(ICountryRepository countryRepository, IMapper mapper)
	{
		_countryRepository = countryRepository;
		_mapper = mapper;
	}

	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
	[ProducesResponseType(400)]
	public IActionResult GetCountries()
	{
		var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		return Ok(countries);
	}

	[HttpGet("{countryId}")]
	[ProducesResponseType(200, Type = typeof(Country))]
	[ProducesResponseType(404)]
	[ProducesResponseType(400)]
	public IActionResult GetCountry(int countryId)
	{
		if (!_countryRepository.CountryExists(countryId))
			return NotFound();

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));

		return Ok(country);
	}

	[HttpGet("country/{ownerId}")]
    [ProducesResponseType(200, Type = typeof(Country))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetCountryByOwnerId(int ownerId)
	{
		var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwnerId(ownerId));

		if (country is null) //nemamo metodu za proveru ownerId-a, imamo za countryId ali to nam ovde ne sluzi
			return NotFound(); // zato radimo ovo rucno

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		return Ok(country);
	}


	// POST
	[HttpPost]
	[ProducesResponseType(200)] // ne mora
	[ProducesResponseType(400)]
	public IActionResult CreateCountry([FromBody] CountryDto newCountry)
	{
		if (newCountry is null)
			return BadRequest(ModelState);

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var country = _countryRepository.GetCountries() //ovo vraca listu normalnu, na dalje radimo Where nad listom! :D
			.Where(c => c.Name.Trim().ToUpper() == newCountry.Name.Trim().ToUpper())
			.FirstOrDefault();

		//ako vrati null znaci da nema te drzave i to je super, ali ako vrati nesto drugo znaci da je ima
		if(country is not null)
		{
			ModelState.AddModelError("", "Country already exists");
			return StatusCode(422, ModelState);
		}

		//ako stignemo ovde mozemo da ubacimo, ali uvek moze mozda neka neocekivana greska da nastane tokom cuvanja
		var countryMapped = _mapper.Map<Country>(newCountry); // zapamti da newCountry hocemo da ubacimo a ne country za proveru da li postoji taj country
		if (!_countryRepository.CreateCountry(countryMapped))
		{ // ako vrati false onda cuvanje nije uspelo
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState); // ovde vracamo 500 jer je error na serveru!
		}

		//Ako dodjemo do ovde sve je uspelo
		return Ok("Successfuly created"); // dakle u Ok moze i poruka :D
	}


	//PUT (Update)
	[HttpPut("{countryId}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto updateCountryDto)
	{
		if (updateCountryDto == null)
			return BadRequest(ModelState);
		
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		if (countryId != updateCountryDto.Id)
			return BadRequest(ModelState);

		if (!_countryRepository.CountryExists(countryId))
			return NotFound();

		var updateCountry = _mapper.Map<Country>(updateCountryDto);

		if (!_countryRepository.UpdateCountry(updateCountry))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}

		return NoContent();
	}

	//Delete
	[HttpDelete("countryId")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	public IActionResult DeleteCountry(int countryId)
	{
		if (!_countryRepository.CountryExists(countryId))
			return NotFound();
		if (!ModelState.IsValid)
			return BadRequest();

		var countryDelete = _countryRepository.GetCountry(countryId);

		if (!_countryRepository.DeleteCountry(countryDelete))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}

		return NoContent();
	}
}
