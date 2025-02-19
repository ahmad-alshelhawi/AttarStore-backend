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
        //public DbSet<Status> Statuses { get; set; }
        public DbSet<PermissionUser> PermissionUser { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        //public DbSet<ContentType> ContentType { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //// User - Department
            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.Department)
            //    .WithMany(d => d.Users)
            //    .HasForeignKey(u => u.DepartmentId)
            //    .OnDelete(DeleteBehavior.NoAction);


            //modelBuilder.Entity<Ticket>()
            //.HasMany(t => t.Attachments)
            //.WithOne(a => a.Ticket)
            //.HasForeignKey(a => a.TicketId);

            //// User - UserAction
            //modelBuilder.Entity<User>()
            //    .HasMany(u => u.UserActions)
            //    .WithOne(ua => ua.User)
            //    .HasForeignKey(ua => ua.UserId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //// UserAction - ActionLog
            //modelBuilder.Entity<UserAction>()
            //    .HasOne(ua => ua.ActionLog)
            //    .WithMany(al => al.UserAction)
            //    .HasForeignKey(ua => ua.ActionLogId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //// UserAction - Admin
            //modelBuilder.Entity<UserAction>()
            //    .HasOne(ua => ua.Admin)
            //    .WithMany(a => a.UserActions)
            //    .HasForeignKey(ua => ua.AdminId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //// Ticket - Status
            ///*modelBuilder.Entity<Ticket>()
            //    .HasOne(t => t.Status)
            //    .WithMany(s => s.Tickets)
            //    .HasForeignKey(t => t.StatusId)
            //    .OnDelete(DeleteBehavior.NoAction);*/

            ///*  // Ticket - Priority
            //  modelBuilder.Entity<Ticket>()
            //      .HasOne(t => t.Priority)
            //      .WithMany(p => p.Tickets)
            //      .HasForeignKey(t => t.PriorityId)
            //      .OnDelete(DeleteBehavior.NoAction);*/

            //// Ticket - User (Creator)
            //modelBuilder.Entity<Ticket>()
            //    .HasOne(t => t.User)
            //    .WithMany(u => u.Tickets)
            //    .HasForeignKey(t => t.UserId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //// PermissionUser - User
            //modelBuilder.Entity<PermissionUser>()
            //    .HasOne(pu => pu.User)
            //    .WithMany(u => u.Permissions)
            //    .HasForeignKey(pu => pu.UserId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //// PermissionUser - Permission
            //modelBuilder.Entity<PermissionUser>()
            //    .HasOne(pu => pu.Permission)
            //    .WithMany(p => p.Users)
            //    .HasForeignKey(pu => pu.PermissionId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //// Admin - UserAction
            //modelBuilder.Entity<Admin>()
            //    .HasMany(a => a.UserActions)
            //    .WithOne(ua => ua.Admin)
            //    .HasForeignKey(ua => ua.AdminId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<Ticket>()
            //    .HasMany(a => a.Replies)
            //    .WithOne(ua => ua.Ticket)
            //    .HasForeignKey(ua => ua.TicketId)
            //    .OnDelete(DeleteBehavior.Cascade);

            ///*// Department - Parent Department (Self-Referencing)
            //modelBuilder.Entity<Department>()
            //    .HasOne(d => d.Parent)
            //    .WithMany(p => p.Departments)
            //    .HasForeignKey(d => d.ParentId)
            //    .OnDelete(DeleteBehavior.NoAction);*/


            /*modelBuilder.Entity<Admin>(e =>
            {
                e.Property(p => p.Name).HasColumnType("varchar(250)");
                e.Property(p => p.Email).HasColumnType("varchar(250)");
                e.HasIndex(p => p.Email).IsUnique();
                e.HasIndex(p => p.Name).IsUnique();
            });

            modelBuilder.Entity<User>(e =>
            {
                e.Property(p => p.Name).HasColumnType("varchar(250)");
                e.Property(p => p.Email).HasColumnType("varchar(250)");
                e.HasIndex(p => p.Email).IsUnique();
                e.HasIndex(p => p.Name).IsUnique();
            });*/




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
            // Seed initial data
            //modelBuilder.Entity<Status>().HasData(
            //    new Status { Id = 1, Name = "Open" },
            //    new Status { Id = 2, Name = "In Progress" },
            //    new Status { Id = 3, Name = "Closed" }
            //);
            //modelBuilder.Entity<Priority>().HasData(
            //    new Priority { Id = 1, Level = "Low" },
            //    new Priority { Id = 2, Level = "Medium" },
            //    new Priority { Id = 3, Level = "Critical" }
            //);
            //modelBuilder.Entity<Department>().HasData(
            //    new Department { Id = 1, Name = "IT" }



            //);


        }

    }
}


