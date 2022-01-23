using bookservice_api.Models;
using Microsoft.EntityFrameworkCore;

namespace DefaultNamespace;

public class BookServiceDBContext : DbContext
{
    public BookServiceDBContext(DbContextOptions<BookServiceDBContext> options) : base(options)
    {
    }
    
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
}