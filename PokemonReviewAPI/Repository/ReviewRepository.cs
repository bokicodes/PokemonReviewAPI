using PokemonReviewAPI.Data;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Repository;

public class ReviewRepository : IReviewRepository
{
    private readonly DataContext _context;

    public ReviewRepository(DataContext context)
    {
        _context = context;
    }

    public Review GetReview(int reviewId)
    {
        return _context.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
    }

    public ICollection<Review> GetReviews()
    {
        return _context.Reviews.ToList();
    }

    public ICollection<Review> GetReviewsOfAPokemon(int pokeId)
    {
        return _context.Reviews.Where(r => r.Pokemon.Id == pokeId).ToList();
        //ovo kada pravis razmisljas o vezama izmedju dva entitia i onda ces lako da znas
        //sta treba da napises :D
    }

    public bool ReviewExists(int reviewId)
    {
        return _context.Reviews.Any(r => r.Id == reviewId);
    }

    public bool CreateReview(Review review)
    {
        _context.Add(review);
        return Save();
    }

    public bool Save()
    {
        return _context.SaveChanges() > 0;
    }

    public bool UpdateReview(Review review)
    {
        _context.Update(review);
        return Save();
    }

    public bool DeleteReview(Review review)
    {
        _context.Remove(review);
        return Save();
    }

    public bool DeleteReviews(ICollection<Review> reviews)
    {
        _context.RemoveRange(reviews);
        return Save();
    }
}
