using System.Collections.Generic;

namespace bookservice_api.DTOs;

public class BookPostDTO
{
    public string Title { get; set; }
    public string AuthorName { get; set; } 
    public int Year { get; set; }
    public decimal Price { get; set; }
    public virtual ICollection<string> GenreNames { get; set; }
}