using System.ComponentModel.DataAnnotations;

namespace telerik.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Publisher { get; set; }
        public string YearPublished { get; set; }
        public string? CoverImage { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public string? InStock => Stock > 0 ? "Available" : "Not Available";
    }
}
