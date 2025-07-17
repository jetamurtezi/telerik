using System;
using System.ComponentModel.DataAnnotations;

namespace telerik.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        public Book Book { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Address { get; set; }

        [Required]
        [Display(Name = "Numri i Kartës")]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Data e Skadencës")]
        public string ExpiryDate { get; set; } 

        [Required]
        [Display(Name = "CVV")]
        [StringLength(4, MinimumLength = 3)]
        public string CVV { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}
