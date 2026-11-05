using Xunit;
using Microsoft.EntityFrameworkCore;
using hmctsBackend.Tests.Helpers;
using hmcts_test.model;
using Task = System.Threading.Tasks.Task;
using TaskEntry = hmcts_test.model.Task;
using TaskEntryStatus = hmcts_test.model.TaskStatus;

namespace hmctsBackend.Tests.Integration.Database
{
    public class TaskDatabaseTests
    {
        [Fact]
        public async Task CreateTask_SaveTaskToDatabase()
        {
            // Arrange
            using var database = new TestDatabase();
            var db = database.Context;

            var newTask = new TaskEntry
            {
                Title = "Integration Test Task",
                Description = "This is a test task for integration testing.",
                Status = TaskEntryStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(3)
            };

            // Act
            db.Tasks.Add(newTask);
            await db.SaveChangesAsync();

            var retrievedTask = await db.Tasks.FirstOrDefaultAsync(t => t.Id == newTask.Id);

            // Assert
            Assert.NotNull(retrievedTask);
            Assert.Equal(newTask.Title, retrievedTask!.Title);
            Assert.Equal(newTask.Description, retrievedTask.Description);
            Assert.Equal(newTask.Status, retrievedTask.Status);
            Assert.Equal(newTask.DueDate, retrievedTask.DueDate);
        }

        [Fact]
        public async Task UpdateTask_ChangesPersisted()
        {
            // Arrange
            using var database = new TestDatabase();
            var db = database.Context;

            var task = new TaskEntry
            {
                Title = "Task to Update",
                Description = "This task will be updated.",
                Status = TaskEntryStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            // Act
            task.Title = "Updated Task Title";
            task.Status = TaskEntryStatus.InProgress;

            await db.SaveChangesAsync();

            // Assert
            db.ChangeTracker.Clear();

            var updatedTask = await db.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            Assert.NotNull(updatedTask);
            Assert.Equal("Updated Task Title", updatedTask!.Title);
            Assert.Equal(TaskEntryStatus.InProgress, updatedTask.Status);
        }

        [Fact]
        public async Task DeleteTask_RemovesTaskFromDatabase()
        {
            // Arrange
            using var database = new TestDatabase();
            var db = database.Context;

            var task = new TaskEntry
            {
                Title = "Task to Delete",
                Description = "This task will be deleted.",
                Status = TaskEntryStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(2)
            };

            db.Add(task);
            await db.SaveChangesAsync();

            // Act
            db.Tasks.Remove(task);
            await db.SaveChangesAsync();

            // Assert
            var deletedTask = await db.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            Assert.Null(deletedTask);
        }
    }
}
