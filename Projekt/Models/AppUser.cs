using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Projekt.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Renting> Rentings { get; set; } = new List<Renting>();
    }
}
