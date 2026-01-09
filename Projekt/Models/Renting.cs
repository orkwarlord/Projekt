using System;

namespace Projekt.Models
{
    public class Renting
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;

        public DateTime RentedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnedAt { get; set; }
    }
}
