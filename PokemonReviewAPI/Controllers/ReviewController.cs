using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewAPI.Dto;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
	private readonly IReviewRepository _reviewRepository;
	private readonly IMapper _mapper;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IPokemonRepository _pokemonRepository;

    public ReviewController(IReviewRepository reviewRepository, IMapper mapper, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository)
	{
		_reviewRepository = reviewRepository;
		_mapper = mapper;
        _reviewerRepository = reviewerRepository;
        _pokemonRepository = pokemonRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
    [ProducesResponseType(400)]
    public IActionResult GetReviews()

    {


        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

          if (!ModelState.IsValid)
            return BadRequest(ModelState);

            return Ok(reviews);
    }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))] // NE MORA OVO, samo cini da API endpoint izgleda malo cleaner, nema neku drugu svrhu
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId)) // eto cemu sluzi ta metoda :D, mogli smo i bez nje pa da ovde proverimo ali ovako je lepse
                return NotFound();

            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(review);
        }

    [HttpGet("review/{pokeId}")]
    [ProducesResponseType(200, Type = typeof(Review))] // NE MORA OVO, samo cini da API endpoint izgleda malo cleaner, nema neku drugu svrhu
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetReviewsForAPokemon(int pokeId)
    {
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));

        if (!reviews.Any())
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviews);
    }


    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokemonId, [FromBody] ReviewDto newReviewDto)
    {
        if (newReviewDto == null)
            return BadRequest(ModelState);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var review = _reviewRepository.GetReviews()
            .Where(r => r.Title.Trim().ToUpper() == newReviewDto.Title.Trim().ToUpper()
            && r.Text.Trim().ToUpper() == newReviewDto.Text.Trim().ToUpper()
            && r.Rating == newReviewDto.Rating)
            .FirstOrDefault();

        if (review != null)
        {
            ModelState.AddModelError("", "Review already exists");
            return StatusCode(422, ModelState);
        }
        
        var newReview = _mapper.Map<Review>(newReviewDto);

        newReview.Reviewer = _reviewerRepository.GetReviewer(reviewerId);
        newReview.Pokemon = _pokemonRepository.GetPokemon(pokemonId);

        if (!_reviewRepository.CreateReview(newReview))
        { //ako ne uspe save
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500,ModelState);
        }

        return Ok("Successfuly created");
    }


    // PUT (Update)
    [HttpPut("{reviewId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updateReviewDto)
    {
        if (updateReviewDto == null)
            return BadRequest(ModelState);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (reviewId != updateReviewDto.Id)
            return BadRequest(ModelState);

        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();

        var updateReview = _mapper.Map<Review>(updateReviewDto);

        if (!_reviewRepository.UpdateReview(updateReview))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    // Delete
    [HttpDelete("{reviewId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeleteReview(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var reviewDelete = _reviewRepository.GetReview(reviewId);

        if (!_reviewRepository.DeleteReview(reviewDelete))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

}
