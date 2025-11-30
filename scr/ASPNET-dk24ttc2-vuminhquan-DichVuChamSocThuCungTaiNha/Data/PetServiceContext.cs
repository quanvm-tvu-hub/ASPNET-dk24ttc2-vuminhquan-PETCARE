using Microsoft.EntityFrameworkCore;
using WebThuCungNew.Models;

namespace WebThuCungNew.Data
{
    public class PetServiceContext : DbContext
    {
        public PetServiceContext(DbContextOptions<PetServiceContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Client)
                .WithMany(u => u.ClientBookings)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Sitter)
                .WithMany(u => u.SitterBookings)
                .HasForeignKey(b => b.SitterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
