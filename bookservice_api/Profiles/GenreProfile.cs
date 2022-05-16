using AutoMapper;
using bookservice_api.Models;
using bookservice_api.DTOs;

namespace bookservice_api.Profiles;

public class GenreProfile : Profile
{
    public GenreProfile()
    {
        // Genre.Books automatically gets mapped to BookDTOs
        CreateMap<Genre, GenreDTO>();  
    }
}