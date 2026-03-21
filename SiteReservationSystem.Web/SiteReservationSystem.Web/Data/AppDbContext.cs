using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Models; // adjust namespace if your models are somewhere else

namespace SiteReservationSystem.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Add a DbSet for each of your tables
        public DbSet<Site> Sites { get; set; }
        public DbSet<SitePhoto> SitePhotos { get; set; }
        public DbSet<SiteType> SiteTypes { get; set; }
        public DbSet<SiteTypePrice> SiteTypePrices { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}