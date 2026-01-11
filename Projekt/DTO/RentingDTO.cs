using Projekt.Models;

namespace Projekt.DTO
{
    public class RentingDTO
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public DateTime RentedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnedAt { get; set; }
        public RentingDTO() { }
        public RentingDTO(Renting renting)
        {
            Id = renting.Id;
            RentedAt = renting.RentedAt;
            ReturnedAt = renting.ReturnedAt;
        }
    }
}