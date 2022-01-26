using System.Collections.Generic;
using System.Threading.Tasks;
using bookservice_api.Models;
using DefaultNamespace;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookservice_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : Controller
{
    private BookServiceDBContext _context;

    public AuthorsController(BookServiceDBContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<Author>> GetAuthors()
    {
        IEnumerable<Author> authors = _context.Authors;
        return Ok(authors);
    }

    [HttpGet("{id}")]
     public async Task<ActionResult<Author>> GetAuthor(int id)
     {
         Author author = await _context.Authors.FindAsync(id);
         if (author == null)
         {
             return NotFound();
         }
    
         return Ok(author);
     }

     [HttpPost]
     public async Task<IActionResult> PostAuthor(Author author)
     {
         if (!ModelState.IsValid)
         {
             return BadRequest(ModelState);
         }
         
         _context.Authors.Add(author);
         await _context.SaveChangesAsync();

         // Tutorial used CreatedAtRoute. Here, could also just use "GetAuthor"
         return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
     }
     
     [HttpPut("{id}")]
     public async Task<IActionResult> PutAuthor(int id, Author author)
     {
         if (!ModelState.IsValid)
         {
             return BadRequest(ModelState);
         }

         if (id != author.Id)
         {
             return BadRequest();
         }

         _context.Entry(author).State = EntityState.Modified;

         // NB have left out DbUpdateConcurrencyException catching here
         await _context.SaveChangesAsync();
         
         return NoContent();
     }
     
     [HttpDelete("{id}")]
     public async Task<IActionResult> DeleteAuthor(int id)
     {
         Author author = await _context.Authors.FindAsync(id);
         if (author == null)
         {
             return NotFound();
         }

         _context.Authors.Remove(author);
         await _context.SaveChangesAsync();

         return Ok(author);
     }
     
}