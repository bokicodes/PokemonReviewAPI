using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewAPI.Dto;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase // daje BadRequest() i sve ostalo
{
	private readonly ICategoryRepository _categoryRepository;
	private readonly IMapper _mapper;

	public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
	{
		_categoryRepository = categoryRepository; //da bi pristupao metodama u ICategoryRepository
		_mapper = mapper;
		// a one su implementirane u categoryrepository pa ce one samo odatle da se pozivaju
	}

	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
	[ProducesResponseType(400)]
	public IActionResult GetCategories()
	{
		var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		return Ok(categories);
	} /*imao sam error 500 ovo mi nije htelo a sve je dobro... Greska je bila
	   to sto nisam u program.cs injectovao interfejs odnosno repository pattern!
		Kada je greska na serveru cesto je problem to sto nisi odradio injection.*/

	[HttpGet("{categoryId}")] //daje detaljniji opis sta trazimo, moze da znaci ljudima!
	[ProducesResponseType(200, Type = typeof(Category))]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	public IActionResult GetCategory(int categoryId) // e ali onda i ovde moras categoryId ili ce da trazi dva Id-a !!!
	{
		if (!_categoryRepository.CategoryExists(categoryId))
			return NotFound();

		var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));

		if (!ModelState.IsValid)
			return BadRequest(ModelState); //moze i bez parametra ja msm

		return Ok(category);
	}

	[HttpGet("pokemon/{categoryId}")]
	[ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	public IActionResult GetPokemonByCategoryId(int categoryId)
	{
		if (!_categoryRepository.CategoryExists(categoryId))
			return NotFound(); //da nemam ovo vratilo bi 200 ali prazno, to nije dobro

		var pokemons = _mapper.Map<List<PokemonDto>>
			(_categoryRepository.GetPokemonsByCategoryId(categoryId));

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		return Ok(pokemons);
	}


	// POST METHOD (CREATE)
	[HttpPost]
	[ProducesResponseType(200)] //HTTP Status 204 (No Content) indicates that the server has successfully fulfilled the request and that there is no content to send in the response payload body.
	[ProducesResponseType(400)]
	public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
	{
		if (categoryCreate == null) // ako user ne posalje nista!
			return BadRequest(ModelState);

		// Sada mora da proverimo da li postoji vec ova kategorija u bazi, ne sme da se doda ista ako vec postoji jer ce da dobijemo server error, dakle to ne sme!
		var category = _categoryRepository.GetCategories()
			.Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.Trim().ToUpper())
			.FirstOrDefault();
		// Razlog zasto radimo po Name je zato sto cemo u API-u da unosimo NAME a ne Id, Id ce da ostavimo da EF sam unese koji treba

		if (category != null) //ako vrati null znaci da nema takva kategorija i to je dobro, ako ne vrati null znaci da je ima!
		{
			ModelState.AddModelError("", "Category already exists"); //AddModelError daje error msg, i ovo je Key-Value pair funkcija, zato ima dva parametra!
																	 //		Mi nemamo Key tako da samo popunjavamo value, poruku a to je drugi parametar!
																	 // U ModelState-u (moj entity kako izgleda u JSONu) doda errormsg i onda ispod vrati ModelState sa status kodom 422 :D
			return StatusCode(422, ModelState); // 422 znaci da je sitnaksa i request dobar ali instrukcije nisu mogle da se izvrse
		}

		//Provera da li ono sto je user poslao je uopste okej
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		// Sada posto nam je parametar CategoryDto, moramo da ga mapujemo u normalan Category da bi 
		// normalan category ubacili u bazu. Obavezno moras i u MappingProfile da napravis novu mapu
		// CreateMap<CategoryDto, Category>();
		var categoryMap = _mapper.Map<Category>(categoryCreate);

		//Finalna stvar, proveravamo da li je metoda uspela odnosno da li je SAVE uspeo
		if (!_categoryRepository.CreateCategory(categoryMap))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState); //500 Internal Server Error server error response code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request
		}

		return Ok("Successfuly created");
	}

	[HttpPut("{categoryId}")] //kad se unese prosledjuje se dole u categoryId parametar :D, uvek matchuj naziv da ne bi doslo do greske neke
	[ProducesResponseType(204)] //mozes i 200 ako hoces da vratis Ok("Successfuly updated")
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updatedCategoryDto)
	{
		if (updatedCategoryDto == null)
			return BadRequest(ModelState);
		
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		if (categoryId != updatedCategoryDto.Id) //mora da se matchuju
			return BadRequest(ModelState);

		if (!_categoryRepository.CategoryExists(categoryId))
			return NotFound();

		//ne moras da proveravas i da li postoji updatedCategoryDto.Id zato sto se gore
		//vec proverava da li se matchuje sa categoryId i onda ako je razlicito izacice,
		//a ako je isto onda je sve jedno sa kojim proveravas jer su isti, mi smo ovde
		//proverili sa categoryId :D

		var updatedCategory = _mapper.Map<Category>(updatedCategoryDto);

		if (!_categoryRepository.UpdateCategory(updatedCategory))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}

		return NoContent(); // moze i Ok("Successfuly updated");

	}


	// DELETE
	[HttpDelete("{categoryId}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	public IActionResult DeleteCategory(int categoryId)
	{
		if (!_categoryRepository.CategoryExists(categoryId))
			return NotFound();

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var categoryDelete = _categoryRepository.GetCategory(categoryId);

		if (!_categoryRepository.DeleteCategory(categoryDelete))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}

		return NoContent();
	}
}
