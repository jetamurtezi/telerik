using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace telerik.Models
{
    public class Book
    {
        [Key]
        public int? Id { get; set; }

        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }

        public int? CategoryId { get; set; }

        public string? CoverImage { get; set; }

        [NotMapped]
        public string? InStock => Stock > 0 ? "Available" : "Not Available";
    }
}
