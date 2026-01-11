using Projekt.Models;

namespace Projekt.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Renting> Rentings { get; set; } 
        public BookDTO() { }
        public BookDTO(Book book)
        {
            Id = book.Id;
            Title = book.Title;
            Author = book.Author;
            Description = book.Description;

        }
    }
}
