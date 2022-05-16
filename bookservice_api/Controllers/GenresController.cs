using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using bookservice_api.DTOs;
using bookservice_api.Models;
using DefaultNamespace;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookservice_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : Controller
{
    private BookServiceDBContext _context;
    private IMapper _mapper;
    
    public GenresController(BookServiceDBContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<GenreDTO>> GetGenres()
    {
        IQueryable<Genre> genres = _context.Genres;
        IQueryable<GenreDTO> dtos = _mapper.ProjectTo<GenreDTO>(genres);
        return Ok(dtos);
    }
    
}