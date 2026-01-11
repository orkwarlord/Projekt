using System;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class Renting
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
    }
}
