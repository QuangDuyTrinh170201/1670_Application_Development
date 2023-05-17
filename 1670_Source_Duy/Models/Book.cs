using System.ComponentModel.DataAnnotations;

namespace _1670_Source_Duy.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string Author { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
