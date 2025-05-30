using System.Reflection.Metadata.Ecma335;
using MarcusRent.Classes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MarcusRent.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CarImage> CarImages { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Many-to-many Order <-> Car
            builder.Entity<Order>()
                .HasMany(o => o.Cars)
                .WithMany(c => c.Orders);
        }
    }
}

