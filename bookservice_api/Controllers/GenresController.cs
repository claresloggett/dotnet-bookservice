using System;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<GenreDTO>> GetGenre(int id)
    {
        // Include, ProjectTo and then SingleOrDefaultAsync() does optimise query
         
        IQueryable<GenreDTO> genreQuery = _mapper.ProjectTo<GenreDTO>(
            _context.Genres
                .Include(g => g.Books));
             
        GenreDTO genre = await genreQuery.SingleOrDefaultAsync(g => g.Id == id);

        if (genre == null)
        {
            return NotFound();
        }
         
        return Ok(genre);
    }
    
    [HttpGet("name/{name}")]
    public async Task<ActionResult<GenreDTO>> GetGenreByName(string name)
    {
        IEnumerable<GenreDTO> genres = await _context.Genres
            .Where(g => g.Name.ToLower()==name.ToLower())
            .Include(g => g.Books)         
            .Select(g => _mapper.Map<GenreDTO>(g))
            .ToListAsync();

        return Ok(genres);
    }

    [HttpPost]
    public async Task<IActionResult> PostGenre(GenrePostDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var (missingBooks, bookList) = FindBooks(dto);

        if (missingBooks.Any())
        {
            string missingBookString = String.Join(",", missingBooks);
            return NotFound($"Books not found in system: {missingBookString}");
        }
        
        var genre = new Genre()
        {
            Name = dto.Name,
            Books = bookList
        };

        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        // Tutorial used CreatedAtRoute. Here, could also just use "GetGenre"
        return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, _mapper.Map<GenreDTO>(genre));
    }
     
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGenre(int id, GenrePostDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        Genre genre = await _context.Genres.FindAsync(id);
        if (genre == null)
        {
            return NotFound("Genre id not found");
        }
        
        var (missingBooks, bookList) = FindBooks(dto);

        if (missingBooks.Any())
        {
            string missingBookString = String.Join(",", missingBooks);
            return NotFound($"Books not found in system: {missingBookString}");
        }

        genre.Name = dto.Name;
        genre.Books = bookList;

        // NB have left out DbUpdateConcurrencyException catching here
        await _context.SaveChangesAsync();
         
        return NoContent();
    }
     
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        Genre genre = await _context.Genres.FindAsync(id);
        if (genre == null)
        {
            return NotFound();
        }

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();

        return Ok(genre);
    }

    // Returns missingBookTitles, foundBooks
    // Note this (,) notation uses ValueTuple, newer than Tuple<>; would need .ToTuple() to convert
    private (IEnumerable<string>, List<Book>) FindBooks(GenrePostDTO dto)
    {
        // Is ToUpper() more DB-safe than ToLower() ?
        List<string> uppercaseTitles = dto.BookTitles.Select(t => t.ToUpper()).ToList();
        List<Book> bookList = _context.Books
            .Where(b => uppercaseTitles.Any(title => b.Title.ToUpper()==title))
            .ToList();

        var missingBooks = dto.BookTitles.Except(bookList.Select(b => b.Title), StringComparer.OrdinalIgnoreCase);
        
        return (missingBooks, bookList);
    }
    
}