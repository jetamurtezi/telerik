using System.ComponentModel.DataAnnotations;

namespace telerik.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ContactName { get; set; }
        public string CompanyName { get; set; }
        public string Country { get; set; }
        public string ContactTitle { get; set; }
    }
}
