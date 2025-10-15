using HotelBookingApp.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelBookingApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Users>>();

            string[] roles = { "Admin", "Client" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Создать администратора по умолчанию
            string adminEmail = "admin@hotel.com";
            string adminPassword = "admin123";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new Users { UserName = adminEmail, Email = adminEmail, Name = "Admin" };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
