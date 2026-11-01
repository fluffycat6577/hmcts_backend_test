using Microsoft.EntityFrameworkCore;

namespace hmcts_test.model
{
    public class TaskContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }

        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Task>()
                .Property(t => t.Status)
                .HasConversion<int>()
                .HasDefaultValue(TaskStatus.Pending);
        }
    }
}
