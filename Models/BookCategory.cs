using System.ComponentModel.DataAnnotations;

namespace telerik.Models
{
    public class BookCategory
    {
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
    }
}
