using Microsoft.EntityFrameworkCore;
using PokemonReviewAPI.Data;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Repository;

public class ReviewerRepository : IReviewerRepository
{
	private readonly DataContext _context;

	public ReviewerRepository(DataContext context)
	{
		_context = context;
	}

	public Reviewer GetReviewer(int reviewerId)
	{
		return _context.Reviewers.Where(r => r.Id == reviewerId)
			.Include(e => e.Reviews).FirstOrDefault();
		//ne mora include, namerno je ubacio da vidis sta radi, prikazace i Review navigation property od Reviewer-a
	}

	public ICollection<Reviewer> GetReviewers()
	{
		return _context.Reviewers.ToList();
	}

	public ICollection<Review> GetReviewsByReviewer(int reviewerId)
	{
		return _context.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
	}

	public bool ReviewerExists(int reviewerId)
	{
		return _context.Reviewers.Any(r => r.Id == reviewerId);
	}

    public bool CreateReviewer(Reviewer reviewer)
    {
		_context.Add(reviewer);
		return Save();
    }
    public bool Save()
	{
		return _context.SaveChanges() > 0;
	}

	public bool UpdateReviewer(Reviewer reviewer)
	{
		_context.Update(reviewer);
		return Save();
	}

	public bool DeleteReviewer(Reviewer reviewer)
	{
		_context.Remove(reviewer);
		return Save();
	}
}
