using System.Reflection.Emit;
using MarcusRent.Classes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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

        builder.Entity<Car>()
            .Property(c => c.PricePerDay)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .Property(o => o.Price)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .HasOne(o => o.Car)
            .WithMany()
            .HasForeignKey(o => o.CarId)
            .OnDelete(DeleteBehavior.Restrict); // valfritt men bra praxis
    }
}

