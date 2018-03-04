using Amplifier.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Amplifier.Data
{
    public class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {

            var roles = await CreateRolesAsync(context);
            var admin = await CreateAdminUser(context);
            await AssignRoles(userManager, admin.Email, "Administrator");

            await context.SaveChangesAsync();

        }

        private static async Task<string[]> CreateRolesAsync(ApplicationDbContext context)
        {
            string[] roles = new string[] { "Administrator", "Subscriber", "User" };

            foreach (string role in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);

                if (!context.Roles.Any(r => r.Name == role))
                {
                    await roleStore.CreateAsync(new IdentityRole(role));
                }
            }
            return roles;
        }

        private static async Task<ApplicationUser> CreateAdminUser(ApplicationDbContext context)
        {
            var user = new ApplicationUser
            {
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                PhoneNumber = "+111111111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };


            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "admin");
                user.PasswordHash = hashed;
                

                var userStore = new UserStore<ApplicationUser>(context);
                var result = userStore.CreateAsync(user);

            }
            return user;
        }




        public static async Task<IdentityResult> AssignRoles(UserManager<ApplicationUser> userManager, string email, string role)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(email);
            var roles = new string[] {role};
            var result = await userManager.AddToRolesAsync(user, roles);
            return result;
        }

    }
}
