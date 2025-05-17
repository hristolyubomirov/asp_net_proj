using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CandyShopp.Models;

namespace CandyShopp.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            if (await roleManager.RoleExistsAsync("Admin"))
            {
                return;
            }

            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));

            var adminUser = new ApplicationUser
            {
                UserName = "admin@admin",
                Email = "admin@admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "admin");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product 
                    { 
                        Name = "Кекс", 
                        Description = "Вкусен кекс с шоколадово покритие и пълнеж от крем", 
                        Price = 5.99m, 
                        StockQuantity = 10,
                        ImageUrl = "/images/products/keks2.jpg"
                    },
                    new Product 
                    { 
                        Name = "Шоколадови бонбони", 
                        Description = "Млечен шоколадови бонбони с лешници", 
                        Price = 3.99m, 
                        StockQuantity = 20,
                        ImageUrl = "/images/products/shok1.jpg"
                    },
                    new Product 
                    { 
                        Name = "Ванилови бисквити", 
                        Description = "Ванилови бисквити с шоколадово покритие", 
                        Price = 2.99m, 
                        StockQuantity = 15,
                        ImageUrl = "/images/products/candies4.jpg"
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
} 