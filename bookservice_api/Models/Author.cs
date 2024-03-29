using System.ComponentModel.DataAnnotations;

namespace bookservice_api.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual Country Country { get; set; }
    }
}