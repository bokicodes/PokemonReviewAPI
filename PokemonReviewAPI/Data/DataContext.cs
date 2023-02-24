using Microsoft.EntityFrameworkCore;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Data;

public class DataContext : DbContext
{
	public DataContext(DbContextOptions<DataContext> options) : base(options)
	{
	}

	public DbSet<Category> Categories { get; set; }
	public DbSet<Country> Countries { get; set; }
	public DbSet<Owner> Owners { get; set; }
	public DbSet<Pokemon> Pokemons { get; set; }
	public DbSet<PokemonOwner> PokemonOwners { get; set; }
	public DbSet<PokemonCategory> PokemonCategories { get; set; }
	public DbSet<Review> Reviews { get; set; }
	public DbSet<Reviewer> Reviewers { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		//setting up PokemonCategory many to many relationship
		modelBuilder.Entity<PokemonCategory>().
			HasKey(pc => new { pc.PokemonId, pc.CategoryId }); //ovako se rade dva primarna kljuca
		modelBuilder.Entity<PokemonCategory>().
			HasOne(p => p.Pokemon)
			.WithMany(pc => pc.PokemonCategories)
			.HasForeignKey(p => p.PokemonId);
		modelBuilder.Entity<PokemonCategory>().
			HasOne(c => c.Category)
			.WithMany(pc => pc.PokemonCategories)
			.HasForeignKey(c => c.CategoryId);

        //setting up PokemonOwner many to many relationship
        modelBuilder.Entity<PokemonOwner>().
            HasKey(po => new { po.PokemonId, po.OwnerId }); //ovako se rade dva primarna kljuca
        modelBuilder.Entity<PokemonOwner>().
            HasOne(p => p.Pokemon)
            .WithMany(po => po.PokemonOwners)
            .HasForeignKey(p => p.PokemonId);
        modelBuilder.Entity<PokemonOwner>().
            HasOne(o => o.Owner)
            .WithMany(po => po.PokemonOwners)
            .HasForeignKey(o => o.OwnerId);

    }
}
