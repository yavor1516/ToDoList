using Microsoft.EntityFrameworkCore;
using To_Do_List.Models;
using Task = To_Do_List.Models.Task;

namespace To_Do_List
{
    public class TaskContext : DbContext
    {
        public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the foreign key relationship
            modelBuilder.Entity<Task>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed a default user
            var defaultUser = new User
            {
                UserId = 1,
                Email = "default@example.com",
                Password = HashPassword("defaultpassword"),
                FullName = "Default User",
                CreatedAt = DateTime.Now
            };

            modelBuilder.Entity<User>().HasData(defaultUser);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
