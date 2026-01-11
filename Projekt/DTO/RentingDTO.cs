using System.ComponentModel.DataAnnotations;
using Projekt.Models;

namespace Projekt.DTO
{
    public class RentingDTO
    {
        public int Id { get; set; }
        [Display(Name = "Book")]
        public int BookId { get; set; }
        [Display(Name = "Book")]
        public Book Book { get; set; } = null!;

        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = false)]
        public DateTime RentedAt { get; set; } = DateTime.UtcNow;
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = false)]
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