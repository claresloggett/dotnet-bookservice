using AutoMapper;
using bookservice_api.Models;
using bookservice_api.DTOs;

namespace bookservice_api.Profiles;

public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        CreateMap<Author, AuthorDTO>();
    }
}