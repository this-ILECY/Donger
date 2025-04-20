using Microsoft.EntityFrameworkCore;
using Donger.Models; // Make sure to include your models namespace
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Add this using directive
using Microsoft.AspNetCore.Identity; // Add this using directive

namespace Donger.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet for each of our entities
        public DbSet<Person> People { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SubInvoice> SubInvoices { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the many-to-many relationship between Invoice and Person (Debtors)
            // EF Core will create a joining table automatically for this.
            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.Debtors)
                .WithMany(p => p.InvoicesAttended);

            // You can add further configurations here if needed,
            // but the one-to-many relationships are typically
            // configured by convention based on your entity definitions.

            // Example of potential additional configuration (optional):
            // Ensure decimal properties have specific precision and scale
            // modelBuilder.Entity<Product>()
            //     .Property(p => p.Price)
            //     .HasColumnType("decimal(18, 2)");

            // modelBuilder.Entity<SubInvoice>()
            //     .Property(s => s.TotalPrice)
            //     .HasColumnType("decimal(18, 2)");

            // modelBuilder.Entity<Invoice>()
            //     .Property(i => i.TotalPrice)
            //     .HasColumnType("decimal(18, 2)");

            base.OnModelCreating(modelBuilder); // Call the base method
        }
    }
}