using System.Collections.Generic;

namespace bookservice_api.DTOs;

public class BookDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    
    public virtual ICollection<string> Genres { get; set; }
}