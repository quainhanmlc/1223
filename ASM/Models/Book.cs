using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public string? Picture { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
