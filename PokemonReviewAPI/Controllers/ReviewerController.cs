using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewAPI.Dto;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;
using PokemonReviewAPI.Repository;

namespace PokemonReviewAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewerController : ControllerBase
{
	private readonly IReviewerRepository _reviewerRepository;
	private readonly IMapper _mapper;

	public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
	{
		_reviewerRepository = reviewerRepository;
		_mapper = mapper;
	}

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewers()

    {
        var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviewers);
    }

    [HttpGet("{reviewerId}")]
    [ProducesResponseType(200, Type = typeof(Reviewer))] // NE MORA OVO, samo cini da API endpoint izgleda malo cleaner, nema neku drugu svrhu
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetReviewer(int reviewerId)
    {
        if (!_reviewerRepository.ReviewerExists(reviewerId)) // eto cemu sluzi ta metoda :D, mogli smo i bez nje pa da ovde proverimo ali ovako je lepse
            return NotFound();

        var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviewer);
    }

    [HttpGet("reviews/{reviewerId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))] 
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetReviewsByAReviewer(int reviewerId)
    {
        if (!_reviewerRepository.ReviewerExists(reviewerId))
            return NotFound();

        var reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviews);
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult CreateReviewer([FromBody] ReviewerDto newReviewerDto)
    {
        if (newReviewerDto == null)
            return BadRequest(ModelState);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var reviewer = _reviewerRepository.GetReviewers()
            .Where(r => r.FirstName.Trim().ToUpper() + r.LastName.Trim().ToUpper()
            == newReviewerDto.FirstName.Trim().ToUpper() + newReviewerDto.LastName.Trim().ToUpper())
            .FirstOrDefault();

        if(reviewer != null)
        {
            ModelState.AddModelError("", "Reviewer already exists");
            return StatusCode(422, ModelState);
        }

        var newReviewer = _mapper.Map<Reviewer>(newReviewerDto);

        if (!_reviewerRepository.CreateReviewer(newReviewer))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfuly created");
    }


    // PUT (Update)
    [HttpPut("{reviewerId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updateReviewerDto)
    {
        if (updateReviewerDto is null)
            return BadRequest(ModelState);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (reviewerId != updateReviewerDto.Id)
            return BadRequest(ModelState);

        if (!_reviewerRepository.ReviewerExists(reviewerId))
            return NotFound();

        var updateReviewer = _mapper.Map<Reviewer>(updateReviewerDto);

        if (!_reviewerRepository.UpdateReviewer(updateReviewer))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500,ModelState);
        }

        return NoContent();
    }

    // Delete
    [HttpDelete("{reviewerId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeleteReviewer(int reviewerId)
    {
        if (!_reviewerRepository.ReviewerExists(reviewerId))
            return NotFound();
        if (!ModelState.IsValid)
            return BadRequest();

        var reviewerDelete = _reviewerRepository.GetReviewer(reviewerId);

        if (!_reviewerRepository.DeleteReviewer(reviewerDelete))
        {
            ModelState.AddModelError("", "Something went wrong while deleting reviewer");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
}
