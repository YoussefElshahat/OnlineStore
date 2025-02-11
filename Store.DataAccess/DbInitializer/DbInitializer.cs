using JOStore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using Store.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        AppDbContext _appDbContext;

        public DbInitializer(UserManager<IdentityUser> userManager
            ,RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appDbContext = appDbContext;
        }
        public void Initialize()
        {
            try
            {        
                // Apply pending migrations if any
                if (_appDbContext.Database.GetPendingMigrations().Any())
                {
                    Console.WriteLine("Applying migrations...");
                    _appDbContext.Database.Migrate();
                    Console.WriteLine("Migrations applied successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during database initialization: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return;
            }

            // Create roles if they do not exist
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                Console.WriteLine("Creating roles...");
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                Console.WriteLine("Roles created successfully.");
            }

            // Ensure the admin user is created
            if (_userManager.FindByEmailAsync("admin@store.com").GetAwaiter().GetResult() == null)
            {
                Console.WriteLine("Creating admin user...");
                var result = _userManager.CreateAsync(new AppUser
                {
                    UserName = "admin@store.com",
                    Email = "admin@store.com",
                    Name = "Youssef",
                    PhoneNumber = "1234567890",
                    StreetAddress = "ST05",
                    Region = "Madrid",
                    PostalCode = "12345",
                    City = "Madrid",
                }, "Admin1.").GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    AppUser appUser = _appDbContext.AppUsers.FirstOrDefault(u => u.Email == "admin@store.com");
                    if (appUser != null)
                    {
                        _userManager.AddToRoleAsync(appUser, SD.Role_Admin).GetAwaiter().GetResult();
                        Console.WriteLine("Admin user created and added to Admin role.");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to create admin user. Errors:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Admin user already exists. Skipping admin user creation.");
            }

            Console.WriteLine("Database initialization completed.");
        }

    }
}
