using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public BooksController(BookServiceDBContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<Book>> GetBooks()
    {
        IQueryable<Book> books = _context.Books;
        IQueryable<BookDTO> dtos = from b in books
            select new BookDTO()
            {
                Id = b.Id,
                Title = b.Title,
                AuthorName = b.Author.Name
            };
        //.Include(b => b.Author);
        return Ok(dtos);
    }
    
    [HttpGet("{id}")]
         public async Task<ActionResult<Book>> GetBook(int id)
         {
             Book book = await _context.Books.FindAsync(id);
             if (book == null)
             {
                 return NotFound();
             }
        
             return Ok(book);
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