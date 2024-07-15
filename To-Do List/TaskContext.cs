using Microsoft.EntityFrameworkCore;
using To_Do_List.Models;
using Task = To_Do_List.Models.Task;

namespace To_Do_List
{
    public class TaskContext : DbContext
    {
        public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

        public DbSet<Task> Tasks { get; set; }
    }
}
