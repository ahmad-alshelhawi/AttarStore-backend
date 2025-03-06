using AttarStore.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttarStore.Services
{

    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Clients> Clients { get; set; }
        public DbSet<PermissionUser> PermissionUser { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        //public DbSet<ContentType> ContentType { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();




            // Initialize the TokenService or use a static method to generate the refresh token
            var tokenService = new TokenService(null); // Pass null or a valid IConfiguration if required


            var adminPassword = BCrypt.Net.BCrypt.HashPassword("password");
            string refreshToken = null;
            DateTime refreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(1); // Set expiration time

            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Name = "admin",
                    Password = adminPassword,
                    Email = "admin@gmail.com",
                    Phone = "096654467",
                    RefreshToken = refreshToken, // Set generated refresh token
                    RefreshTokenExpiryTime = refreshTokenExpiryTime // Set token expiry time
                }

            );



        }

    }
}


