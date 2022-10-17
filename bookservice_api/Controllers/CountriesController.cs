
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
public class CountriesController : Controller
{
    private BookServiceDBContext _context;
    private IMapper _mapper;
    
    public CountriesController(BookServiceDBContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    [HttpGet("byBook/{bookId}")]
    public ActionResult<IEnumerable<CountryDTO>> GetCountryByBook(int bookId)
    {
        // Get Country via Book -> Author -> Country to test query behaviour
        
        // This unnecessarily fetches the entire Book object
        Book book = _context.Books.SingleOrDefault(b => b.Id == bookId);

        if (book == null)
        {
            return NotFound();
        }
        
        // This statement results in two separate queries which fetch the entire
        // Author object, then entire Country object
        CountryDTO countryDto = _mapper.Map<CountryDTO>(book.Author.Country);
        return Ok(countryDto);
    }
    
    [HttpGet("byBookBetter/{bookId}")]
    public ActionResult<IEnumerable<CountryDTO>> GetCountryByBookBetter(int bookId)
    {
        // Get Country via Book -> Author -> Country to test query behaviour

        // Assuming we want to check and report missing bookId first
        // Could be skipped if we are happy with country==null test below
        int foundBooks = _context.Books.Where(b => b.Id == bookId).Count();
        if (foundBooks < 1)
        {
            return NotFound($"Book with id {bookId} not found");
        }
        
        Country country = _context.Books
            .Include(b => b.Author)
            .ThenInclude(a => a.Country)
            .Where(b => b.Id == bookId)
            .Select(b => b.Author.Country)
            .SingleOrDefault();
        
        if (country == null)
        {
            return NotFound();
        }
        
        CountryDTO countryDto = _mapper.Map<CountryDTO>(country);
        return Ok(countryDto);
    }
}

