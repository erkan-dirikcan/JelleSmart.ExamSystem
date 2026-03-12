using JelleSmart.ExamSystem.Core.Entities.Identity;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedAdminUserAsync(userManager, roleManager);
        }

        private static async Task SeedRolesAsync(RoleManager<AppRole> roleManager)
        {
            string[] roles = { UserRoles.Admin, UserRoles.Teacher, UserRoles.Student };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var appRole = new AppRole
                    {
                        Name = role,
                        Description = role switch
                        {
                            UserRoles.Admin => "Tam yetkili yönetici",
                            UserRoles.Teacher => "Ders ve sınav sorumlusu",
                            UserRoles.Student => "Sınav çözen öğrenci",
                            _ => ""
                        }
                    };

                    var result = await roleManager.CreateAsync(appRole);

                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            // Check if any user with Admin role exists
            var adminUsers = await userManager.GetUsersInRoleAsync(UserRoles.Admin);

            if (adminUsers.Any())
            {
                // At least one admin exists, no need to create bootstrap admin
                return;
            }

            // No admin exists, create bootstrap admin
            const string adminEmail = "info@jellosmart.com";
            const string adminPassword = "Orko123!";

            var adminUser = new AppUser
            {
                FirstName = "Jello",
                LastName = "Smart",
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create bootstrap admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Ensure Admin role exists before assigning
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new AppRole
                {
                    Name = UserRoles.Admin,
                    Description = "Tam yetkili yönetici"
                });
            }

            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
        }
    }
}
