using System.Collections.Generic;
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
    public ActionResult<IEnumerable<Author>> GetAuthors()
    {
        IEnumerable<Book> books = _context.Books;   //.Include(b => b.Author);
        return Ok(books);
    }
}