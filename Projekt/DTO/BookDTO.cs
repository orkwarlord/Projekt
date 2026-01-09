namespace Projekt.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
