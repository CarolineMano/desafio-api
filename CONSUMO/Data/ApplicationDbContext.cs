using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CONSUMO.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.SeedUsers(builder);
        }

        private void SeedUsers(ModelBuilder builder)
        {
            var hasher = new PasswordHasher<IdentityUser>();

            IdentityUser user1 = new IdentityUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "henrique@email.com",
                Email = "henrique@email.com",
                NormalizedUserName = "HENRIQUE@EMAIL.COM",
                NormalizedEmail = "HENRIQUE@EMAIL.COM",
                PasswordHash = hasher.HashPassword(null, "Gft2021"),
                EmailConfirmed = true
            };

            builder.Entity<IdentityUser>().HasData(user1);
        }
    }
}