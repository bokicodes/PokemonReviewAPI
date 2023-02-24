using AutoMapper;
using PokemonReviewAPI.Dto;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Kad je Klasa pa KlasaDto to je uglavnom za GET 
            CreateMap<Pokemon, PokemonDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Country, CountryDto>();
            CreateMap<Owner, OwnerDto>();
            CreateMap<Review, ReviewDto>();
            CreateMap<Reviewer, ReviewerDto>();

            // KlasaDto pa Klasa je uglavnom za POST, PUT
            CreateMap<CategoryDto, Category>();
            CreateMap<CountryDto, Country>();
            CreateMap<OwnerDto, Owner>();
            CreateMap<PokemonDto, Pokemon>();
            CreateMap<ReviewDto, Review>();
            CreateMap<ReviewerDto, Reviewer>();
        }
    }
}
