using Projekt.Models;

namespace Projekt.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Book> Books { get; set; }
        public CategoryDTO() { }
        public CategoryDTO(Category category)
        {
            Id = category.Id;
            Name = category.Name;
            Books = category.Books;
        }
    }
}
