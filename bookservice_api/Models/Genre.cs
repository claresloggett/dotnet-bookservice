using System.Collections.Generic;

namespace bookservice_api.Models;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Book> Books { get; set; }
}