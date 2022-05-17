using System.Collections.Generic;

namespace bookservice_api.DTOs;

public class GenrePostDTO
{
    public string Name { get; set; }
    public virtual ICollection<string> BookTitles { get; set; }
}