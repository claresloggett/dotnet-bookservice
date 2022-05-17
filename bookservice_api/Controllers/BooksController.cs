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
    public async Task<IActionResult> PostBook(Book book)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
         
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Tutorial used CreatedAtRoute. Here, could also just use "GetBook"
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }
     
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, Book book)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != book.Id)
        {
            return BadRequest();
        }

        _context.Entry(book).State = EntityState.Modified;

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
}