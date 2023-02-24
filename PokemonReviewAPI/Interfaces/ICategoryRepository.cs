using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Interfaces;

public interface ICategoryRepository
{
    ICollection<Category> GetCategories();
    Category GetCategory(int id);
    ICollection<Pokemon> GetPokemonsByCategoryId(int categoryId); //moze biti vise pokemona sa istom kategorijom zato lista
    bool CategoryExists(int id);
    bool CreateCategory(Category category);
    bool UpdateCategory(Category category);
    bool DeleteCategory(Category category);
    bool Save();
}
