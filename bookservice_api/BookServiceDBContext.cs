using Microsoft.EntityFrameworkCore;

namespace DefaultNamespace;

public class BookServiceDBContext : DbContext
{
    public BookServiceDBContext(DbContextOptions<BookServiceDBContext> options) : base(options)
    {
    }
    
    
}