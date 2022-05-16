using System.Collections.Generic;

namespace bookservice_api.DTOs;

public class GenreDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<BookDTO> Books { get; set; }
}