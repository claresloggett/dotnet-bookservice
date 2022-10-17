using System.ComponentModel.DataAnnotations;

namespace bookservice_api.Models
{
    public class Country
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}