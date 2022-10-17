using AutoMapper;
using bookservice_api.Models;
using bookservice_api.DTOs;

namespace bookservice_api.Profiles;

public class CountryProfile : Profile
{
    public CountryProfile()
    {
        CreateMap<Country, CountryDTO>();
    }
}