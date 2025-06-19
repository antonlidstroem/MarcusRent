using MarcusRent.Classes;
using MarcusRent.Repositories;
using MarcusRental2.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
        var userRepository = serviceProvider.GetRequiredService<IApplicationUserRepository>();

        await SeedRolesAsync(roleManager);
        var users = await SeedUsersAsync(userRepository, userManager);
        var cars = await SeedCarsAsync(carRepository);
        await SeedOrdersAsync(orderRepository, users, cars);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task<List<ApplicationUser>> SeedUsersAsync(IApplicationUserRepository userRepository, UserManager<ApplicationUser> userManager)
    {
        var users = await userRepository.GetAllUsersAsync();

        // Lägg till admin om ingen finns
        bool adminExists = false;
        foreach (var user in users)
        {
            if (await userManager.IsInRoleAsync(user, "Admin"))
            {
                adminExists = true;
                break;
            }
        }

        if (!adminExists)
        {
            await userRepository.AddAsync("admin@example.com", "Admin123!", "Admin");
        }

        // Lägg till testanvändare om inga alls finns
        if (!users.Any())
        {
            for (int i = 0; i < 3; i++)
            {
                await userRepository.AddAsync($"user{i}@example.com", $"User{i}123!", "User");
            }
        }

        return await userRepository.GetAllUsersAsync();
    }

    private static async Task<List<Car>> SeedCarsAsync(ICarRepository carRepository)
    {
        var cars = await carRepository.GetAllAsync();
        if (cars.Any())
            return cars;

        var newCars = new List<Car>
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

        foreach (var car in newCars)
        {
            await carRepository.AddAsync(car);
        }

        return await carRepository.GetAllAsync();
    }

    private static async Task SeedOrdersAsync(IOrderRepository orderRepository, List<ApplicationUser> users, List<Car> cars)
    {
        var existingOrders = await orderRepository.GetAllOrdersAsync();
        if (existingOrders.Any() || users.Count < 3 || cars.Count < 5)
            return;

        var orders = new List<Order>
        {
            new Order
            {
                Customer = users[0],
                UserId = users[0].Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                Price = cars[0].PricePerDay * 3,
                ActiveOrder = true,
                Car = cars[0]
            },
            new Order
            {
                Customer = users[1],
                UserId = users[1].Id,
                StartDate = DateTime.Today.AddDays(-10),
                EndDate = DateTime.Today.AddDays(-7),
                Price = cars[2].PricePerDay * 3,
                ActiveOrder = false,
                Car = cars[2]
            },
            new Order
            {
                Customer = users[2],
                UserId = users[2].Id,
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(10),
                Price = cars[3].PricePerDay * 5,
                ActiveOrder = true,
                Car = cars[3]
            },
            new Order
            {
                Customer = users[0],
                UserId = users[0].Id,
                StartDate = DateTime.Today.AddDays(-3),
                EndDate = DateTime.Today,
                Price = cars[1].PricePerDay * 3,
                ActiveOrder = false,
                Car = cars[1]
            },
            new Order
            {
                Customer = users[1],
                UserId = users[1].Id,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(4),
                Price = cars[4].PricePerDay * 3,
                ActiveOrder = true,
                Car = cars[4]
            }
        };

        foreach (var order in orders)
        {
            await orderRepository.AddOrderAsync(order);
        }
    }
}
