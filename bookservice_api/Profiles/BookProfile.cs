using AutoMapper;
using bookservice_api.Models;
using bookservice_api.DTOs;

namespace bookservice_api.Profiles;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookDTO>()
            .ForMember(
                dest => dest.AuthorName,
                opt => opt.MapFrom(src => src.Author.Name));
    }
}