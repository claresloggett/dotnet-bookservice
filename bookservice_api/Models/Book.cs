using System.ComponentModel.DataAnnotations;

namespace bookservice_api.Models;

public class Book
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string Genre { get; set; }

    // Foreign Key
    public int AuthorId { get; set; }
    // Navigation property
    // With UseLazyLoadingProxies this MUST be virtual, or errors out (i.e. laziness cannot be piecemeal?)
    public virtual Author Author { get; set; }
}