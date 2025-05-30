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

        public DbSet<CarOrder> CarOrders { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CarOrder>()
                .HasKey(co => new { co.CarId, co.OrderId });

            builder.Entity<CarOrder>()
                .HasOne(co => co.Car)
                .WithMany(c => c.CarOrders)
                .HasForeignKey(co => co.CarId);

            builder.Entity<CarOrder>()
                .HasOne(co => co.Order)
                .WithMany(o => o.CarOrders)
                .HasForeignKey(co => co.OrderId);
        }
    }
}

