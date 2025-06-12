using MarcusRent.Classes;
using MarcusRent.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // 1. Skapa roller
        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Skapa adminanvändare
        var adminEmail = "admin@example.com";
        var adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 3. Skapa vanliga användare
        var users = new List<ApplicationUser>();

        for (int i = 1; i <= 3; i++)
        {
            var userEmail = $"user{i}@example.com";
            var userPassword = $"User{i}123!";

            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                user = new ApplicationUser { UserName = userEmail, Email = userEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, userPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
            users.Add(user);
        }

        // 4. Skapa bilar
        if (!context.Cars.Any())
        {
            var cars = new List<Car>
            {
                new Car {
                    Brand = "Volvo", Model = "XC60", Year = 2020, PricePerDay = 800, Available = true,
                    CarImages = new List<CarImage> {
                        new CarImage { Url = "https://example.com/images/volvo-xc60-1.jpg" },
                        new CarImage { Url = "https://example.com/images/volvo-xc60-2.jpg" }
                    }
                },
                new Car {
                    Brand = "Tesla", Model = "Model 3", Year = 2022, PricePerDay = 1200, Available = true,
                    CarImages = new List<CarImage> {
                        new CarImage { Url = "https://example.com/images/tesla-model3-1.jpg" },
                        new CarImage { Url = "https://example.com/images/tesla-model3-2.jpg" }
                    }
                },
                new Car {
                    Brand = "BMW", Model = "i3", Year = 2019, PricePerDay = 750, Available = true,
                    CarImages = new List<CarImage> {
                        new CarImage { Url = "https://example.com/images/bmw-i3-1.jpg" },
                        new CarImage { Url = "https://example.com/images/bmw-i3-2.jpg" }
                    }
                },
                new Car {
                    Brand = "Volkswagen", Model = "Golf", Year = 2021, PricePerDay = 600, Available = true,
                    CarImages = new List<CarImage> {
                        new CarImage { Url = "https://example.com/images/vw-golf-1.jpg" },
                        new CarImage { Url = "https://example.com/images/vw-golf-2.jpg" }
                    }
                },
                new Car {
                    Brand = "Toyota", Model = "Corolla", Year = 2018, PricePerDay = 500, Available = true,
                    CarImages = new List<CarImage> {
                        new CarImage { Url = "https://example.com/images/toyota-corolla-1.jpg" },
                        new CarImage { Url = "https://example.com/images/toyota-corolla-2.jpg" }
                    }
                }
            };

            context.Cars.AddRange(cars);
            await context.SaveChangesAsync();
        }

        // 5. Skapa ordrar
        if (!context.Orders.Any())
        {
            var Cars = await context.Cars.ToListAsync();

            var orders = new List<Order>
            {
                new Order
                {
                    Customer = users[0],
                    UserId = users[0].Id,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(3),
                    Price = Cars[0].PricePerDay * 3,
                    ActiveOrder = true,
                    Car = Cars[0]
                },
                new Order
                {
                    Customer = users[1],
                    UserId = users[1].Id,
                    StartDate = DateTime.Today.AddDays(-10),
                    EndDate = DateTime.Today.AddDays(-7),
                    Price = Cars[2].PricePerDay * 3,
                    ActiveOrder = false,
                    Car = Cars[2]
                },
                new Order
                {
                    Customer = users[2],
                    UserId = users[2].Id,
                    StartDate = DateTime.Today.AddDays(5),
                    EndDate = DateTime.Today.AddDays(10),
                    Price = Cars[3].PricePerDay * 5,
                    ActiveOrder = true,
                    Car = Cars[3]
                },
                new Order
                {
                    Customer = users[0],
                    UserId = users[0].Id,
                    StartDate = DateTime.Today.AddDays(-3),
                    EndDate = DateTime.Today,
                    Price = Cars[1].PricePerDay * 3,
                    ActiveOrder = false,
                    Car = Cars[1]
                },
                new Order
                {
                    Customer = users[1],
                    UserId = users[1].Id,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(4),
                    Price = Cars[4].PricePerDay * 3,
                    ActiveOrder = true,
                    Car = Cars[4]
                }
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();
        }
    }
}
