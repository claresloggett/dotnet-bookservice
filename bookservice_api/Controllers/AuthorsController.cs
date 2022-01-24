using System.Collections.Generic;
using bookservice_api.Models;
using DefaultNamespace;
using Microsoft.AspNetCore.Mvc;

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
}