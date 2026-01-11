using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(40)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
