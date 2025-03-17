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
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ActionLog> ActionLogs { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryProduct> InventoryProducts { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
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
                    ResetToken = null,  // ✅ Set to null explicitly
                    ResetTokenExpiry = null, // ✅ Set to null explicitly
                    RefreshToken = refreshToken, // Set generated refresh token
                    RefreshTokenExpiryTime = refreshTokenExpiryTime, // Set token expiry time
                    IsDeleted = false
                }

            );



        }

    }
}


