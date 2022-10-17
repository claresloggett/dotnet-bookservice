using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using bookservice_api.DTOs;
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
    private IMapper _mapper;

    public AuthorsController(BookServiceDBContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<AuthorDTO>> GetAuthors()
    {
        IEnumerable<AuthorDTO> authors = _context.Authors
            .Select(a => _mapper.Map<AuthorDTO>(a))
            .ToList();
        return Ok(authors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDTO>> GetAuthor(int id)
    {
        // Since this DTO draws on no other tables, FindAsync(id) is ok, but otherwise it seems like a bad idea
        AuthorDTO author = _mapper.Map<AuthorDTO>(await _context.Authors.FindAsync(id));
        if (author == null)
        { 
            return NotFound();
        }
         
        return Ok(author);
    }

    [HttpPost]
    public async Task<IActionResult> PostAuthor(AuthorPostDTO dto)
    {
        // As an experiment, this endpoint breaks the pattern by creating the Country if
        // it does not exist
        
        if (!ModelState.IsValid)
        { 
            return BadRequest(ModelState);
        }

        var country = await _context.Countries
            .SingleOrDefaultAsync(c => c.Name == dto.CountryName);

        if (country == null)
        {
            Console.WriteLine($"Country {dto.CountryName} not found; PostAuthor creating Country");
            country = new Country()
            {
                Name = dto.CountryName
            };
            _context.Countries.Add(country);
            //TODO: Really ought to use a transaction to make sure this does not 
            //succeed while the second save fails
            await _context.SaveChangesAsync();
        }
        
        var author = new Author()
        {
            Name = dto.Name,
            Country = country
        };
         
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        // Tutorial used CreatedAtRoute. Here, could also just use "GetAuthor"
        return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, _mapper.Map<AuthorDTO>(author));
    }
     
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAuthor(int id, AuthorPostDTO dto)
    { 
        if (!ModelState.IsValid)
        { 
            return BadRequest(ModelState);
        }

        Author author = await _context.Authors.FindAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        author.Name = dto.Name;
         
        // needed?
        //_context.Entry(author).State = EntityState.Modified;

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