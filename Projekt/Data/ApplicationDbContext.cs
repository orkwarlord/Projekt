using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projekt.Models;


namespace Projekt.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Renting> Rentings => Set<Renting>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //z Book do Category 
            builder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            //z Renting do Book
            builder.Entity<Renting>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Rentings)
                .HasForeignKey(r => r.BookId);

            //z Renting do AppUser
            builder.Entity<Renting>()
                .HasOne(r => r.AppUser)
                .WithMany(u => u.Rentings)
                .HasForeignKey(r => r.AppUserId);
        }
    }
}
