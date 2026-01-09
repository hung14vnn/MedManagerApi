using Microsoft.AspNetCore.Identity;
using MedManagerApi.Models;

namespace MedManagerApi.Data;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in AppRoles.GetAllRoles())
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    public static async Task SeedSuperAdminAsync(
        UserManager<ApplicationUser> userManager, 
        string email = "superadmin@medmanager.com",
        string password = "SuperAdmin@123")
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
            var superAdmin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = "Super",
                LastName = "Admin",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(superAdmin, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, AppRoles.SuperAdmin);
            }
        }
    }
}
