using System.ComponentModel.DataAnnotations;
using Projekt.Models;

namespace Projekt.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(75)]
        public string Author { get; set; } = string.Empty;
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        [Display(Name = "Category")]
        public Category? Category { get; set; }
        public string? CoverImagePath { get; set; }

        public ICollection<Renting> Rentings { get; set; } = new List<Renting>();
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
