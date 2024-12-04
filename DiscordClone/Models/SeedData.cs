using DiscordClone.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


// PASUL 4: Aici creem cele 3 roluri ( User, Mod, Admin ) direct din backend si 3 conturi cu aceste roluri pt testare
// P5 -> program.cs
namespace DiscordClone.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if the database already contains at least one role
                if (context.Roles.Any())
                {
                    return; // Database already contains roles
                }

                // Create roles in the database
                context.Roles.AddRange(
                    new IdentityRole
                    {
                        Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    },
                    new IdentityRole
                    {
                        Id = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                        Name = "Moderator",
                        NormalizedName = "MODERATOR"
                    },
                    new IdentityRole
                    {
                        Id = "2c5e174e-3b0e-446f-86af-483d56fd7212",
                        Name = "User",
                        NormalizedName = "USER"
                    }
                );

                // Create a password hasher for creating hashed passwords
                var hasher = new PasswordHasher<ApplicationUser>();

                // Create users in the database
                context.Users.AddRange(
                    new ApplicationUser
                    {
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb0", // Primary key
                        UserName = "admin@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Admin1!")
                    },
                    new ApplicationUser
                    {
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb1", // Primary key
                        UserName = "moderator@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "MODERATOR@TEST.COM",
                        Email = "moderator@test.com",
                        NormalizedUserName = "MODERATOR@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Moderator1!")
                    },
                    new ApplicationUser
                    {
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb2", // Primary key
                        UserName = "user@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "User1!")
                    }
                );

                // Associate users with roles
                context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210", // Admin role
                        UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"  // Admin user
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211", // Editor role
                        UserId = "8e445865-a24d-4543-a6c6-9443d048cdb1"  // Editor user
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212", // User role
                        UserId = "8e445865-a24d-4543-a6c6-9443d048cdb2"  // User
                    }
                );

                // Save changes to the database
                context.SaveChanges();
            }
        }
    }
}
