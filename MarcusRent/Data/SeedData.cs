using MarcusRent.Classes;
using MarcusRent.Data;
using MarcusRent.Repositories;
using MarcusRental2.Repositories;
using Microsoft.AspNetCore.Identity;
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
        var carRepository = serviceProvider.GetRequiredService<ICarRepository>();
        var orderRepository = serviceProvider.GetRequiredService<IOrderRepository>();
    


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
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                ApprovedByAdmin = true
            };
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
                user = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true,
                    FirstName = $"User{i}",
                    LastName = "Test",
                    ApprovedByAdmin = true
                };
                var result = await userManager.CreateAsync(user, userPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
            users.Add(user);
        }

        // 4. Skapa bilar om inga finns
        var existingCars = await carRepository.GetAllAsync();
        if (!existingCars.Any())
        {
            var cars = new List<Car>
            {
                new Car {
                    Brand = "Bond", Model = "Bug", Year = 1970, PricePerDay = 800, Available = true,
                    CarImages = new List<CarImage> {
                        new CarImage { Url = "https://silodrome.com/wp-content/uploads/2015/05/Bond-Bug-1.jpg" },
                        new CarImage { Url = "https://silodrome.com/wp-content/uploads/2015/05/Bond-Bug-6.jpg" }
                    }
                },
                new Car {
                    Brand = "Volkswagen", Model = "Type 181", Year = 1968, PricePerDay = 1200, Available = true,
                    CarImages = new List<CarImage> {
                        new CarImage { Url = "https://vochomania.mx/wp-content/uploads/2021/01/5d03cceb052ed_AMARILLO_3.jpg" },
                        new CarImage { Url = "https://dealeraccelerate-all.s3.amazonaws.com/ideal/images/3/8/7/387/1922c9af07d9_hd_1973-volkswagen-thing.jpg" }
                    }
                },
                new Car {
                    Brand = "Nissan", Model = "S Cargo", Year = 1989, PricePerDay = 750, Available = true,
                    CarImages = new List<CarImage> {
                        new CarImage { Url = "https://images.honestjohn.co.uk/imagecache/file/crop/1200x800/media/6347067/Nissan~S-Cargo~(1).jpg" },
                        new CarImage { Url = "https://static1.hotcarsimages.com/wordpress/wp-content/uploads/2022/12/sp00400013.jpg" }
                    }
                },
                new Car {
                    Brand = "Fiat", Model = "600 Multipla", Year = 1956, PricePerDay = 600, Available = true,
                    CarImages = new List<CarImage> {
                        new CarImage { Url = "https://stuartparrclassics.com/wp-content/uploads/2018/12/C_04_sRGB_3000x2000-1800x1198.jpg" },
                        new CarImage { Url = "https://stuartparrclassics.com/wp-content/uploads/2018/12/C_05_sRGB_3000x2000-1200x798.jpg" }
                    }
                },
                new Car {
                    Brand = "Corbin", Model = "Sparrow", Year = 1999, PricePerDay = 500, Available = true,
                    CarImages = new List<CarImage> {
                        new CarImage { Url = "https://assets.rebelmouse.io/media-library/color-image-of-a-corbin-sparrow-parked-in-a-profile-position-at-the-rambler-ranch-in-colorado.jpg?id=31856449&width=980" },
                        new CarImage { Url = "https://www.planetcarsz.com/assets/uploads/images/VEICULOS/C/CORBIN/2000_CORBIN_SPARROW_EV/CORBIN_SPARROW_EV_2000_01.jpg" }
                    }
                }
            };

            foreach (var car in cars)
            {
                await carRepository.AddAsync(car);
            }

            existingCars = await carRepository.GetAllAsync(); // Uppdatera listan efter att bilar lagts till
        }

        //5. Skapa ordrar om inga finns
        var existingOrders = await orderRepository.GetAllOrdersAsync();
        if (!existingOrders.Any())
        {
            var orders = new List<Order>
            {
                new Order
                {
                    Customer = users[0],
                    UserId = users[0].Id,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(3),
                    Price = existingCars[0].PricePerDay * 3,
                    ActiveOrder = true,
                    Car = existingCars[0]
                },
                new Order
                {
                    Customer = users[1],
                    UserId = users[1].Id,
                    StartDate = DateTime.Today.AddDays(-10),
                    EndDate = DateTime.Today.AddDays(-7),
                    Price = existingCars[2].PricePerDay * 3,
                    ActiveOrder = false,
                    Car = existingCars[2]
                },
                new Order
                {
                    Customer = users[2],
                    UserId = users[2].Id,
                    StartDate = DateTime.Today.AddDays(5),
                    EndDate = DateTime.Today.AddDays(10),
                    Price = existingCars[3].PricePerDay * 5,
                    ActiveOrder = true,
                    Car = existingCars[3]
                },
                new Order
                {
                    Customer = users[0],
                    UserId = users[0].Id,
                    StartDate = DateTime.Today.AddDays(-3),
                    EndDate = DateTime.Today,
                    Price = existingCars[1].PricePerDay * 3,
                    ActiveOrder = false,
                    Car = existingCars[1]
                },
                new Order
                {
                    Customer = users[1],
                    UserId = users[1].Id,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(4),
                    Price = existingCars[4].PricePerDay * 3,
                    ActiveOrder = true,
                    Car = existingCars[4]
                }
            };

            foreach (var order in orders)
            {
                await orderRepository.AddOrderAsync(order);
            }
        }
    }
}
