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
public class BooksController : Controller
{
    private BookServiceDBContext _context;
    private IMapper _mapper;
    
    public BooksController(BookServiceDBContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<BookDTO>> GetBooks()
    {
        IQueryable<Book> books = _context.Books;
        IQueryable<BookDTO> dtos = _mapper.ProjectTo<BookDTO>(books);
        return Ok(dtos);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDTO>> GetBook(int id)
    {
        // Include, ProjectTo and then SingleOrDefaultAsync() does optimise query
         
        IQueryable<BookDTO> bookQuery = _mapper.ProjectTo<BookDTO>(_context.Books
            .Include(b => b.Author));
             
        BookDTO book = await bookQuery.SingleOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }
         
        return Ok(book);
    }
     
    [HttpGet("title/{titleString}")]
    public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksByTitle(string titleString)
    {
        IEnumerable<BookDTO> books = await _context.Books
            .Where(b => b.Title.Contains(titleString))
            .Include(b => b.Author)         // Looks like Include() is needed for join, even with projection
            .Select(b => _mapper.Map<BookDTO>(b))
            .ToListAsync();

        return Ok(books);
    }

    [HttpPost]
    public async Task<IActionResult> PostBook(BookPostDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Author author = await _context.Authors
            .Where(a => a.Name == dto.AuthorName)
            .SingleOrDefaultAsync();

        if (author == null)
        {
            return NotFound($"Author {dto.AuthorName} not found");
        }
            
        var (missingGenres, genreList) = FindGenres(dto);

        if (missingGenres.Any())
        {
            string missingGenreString = String.Join(",", missingGenres);
            return NotFound($"Books not found in system: {missingGenreString}");
        }
        
        var book = new Book()
        {
            Title = dto.Title,
            Year = dto.Year,
            Price = dto.Price,
            Author = author,
            Genres = genreList
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Tutorial used CreatedAtRoute. Here, could also just use "GetBook"
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, _mapper.Map<BookDTO>(book));
    }
     
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, BookPostDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Book book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound("Book id not found");
        }
        
        Author author = await _context.Authors
            .Where(a => a.Name == dto.AuthorName)
            .SingleOrDefaultAsync();
        if (author == null)
        {
            return NotFound($"Author {dto.AuthorName} not found");
        }
            
        var (missingGenres, genreList) = FindGenres(dto);
        if (missingGenres.Any())
        {
            string missingGenreString = String.Join(",", missingGenres);
            return NotFound($"Books not found in system: {missingGenreString}");
        }

        book.Title = dto.Title;
        book.Price = dto.Price;
        book.Year = dto.Year;
        book.Author = author;
        book.Genres = genreList;

        // NB have left out DbUpdateConcurrencyException catching here
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
     
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        Book book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return Ok(book);
    }
    
    // Returns missingBookTitles, foundBooks
    // Note this (,) notation uses ValueTuple, newer than Tuple<>; would need .ToTuple() to convert
    private (IEnumerable<string>, List<Genre>) FindGenres(BookPostDTO dto)
    {
        // Is ToUpper() more DB-safe than ToLower() ?
        List<string> uppercaseTitles = dto.GenreNames.Select(t => t.ToUpper()).ToList();
        List<Genre> genreList = _context.Genres
            .Where(b => uppercaseTitles.Any(title => b.Name.ToUpper()==title))
            .ToList();

        var missingBooks = dto.GenreNames.Except(genreList.Select(g => g.Name), StringComparer.OrdinalIgnoreCase);
        
        return (missingBooks, genreList);
    }
}