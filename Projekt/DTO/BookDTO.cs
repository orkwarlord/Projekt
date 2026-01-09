using System.ComponentModel;

namespace Projekt.DTO
{
    public class BookDTO
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public Category? Category { get; set; }

        
    }
}
