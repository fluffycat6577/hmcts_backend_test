using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using hmcts_test.endpoints.handlers;
using hmcts_test.model;
using hmcts_test.controller;
using hmctsBackend.Tests.Helpers;
using Task = System.Threading.Tasks.Task;
using TaskEntry = hmcts_test.model.Task;
using TaskEntryStatus = hmcts_test.model.TaskStatus;

namespace hmctsBackend.Tests.EndpointTests
{
    public class TaskHandlerTests
    {
        [Fact]
        public async Task GetHandler_ReturnsAllTasks()
        {
            // Arrange
            using var database = new TestDatabase();
            var db = database.Context;

            db.Tasks.AddRange(
                new TaskEntry
                {
                    Title = "Task 1",
                    Description = "Description 1",
                    Status = TaskEntryStatus.Pending,
                    DueDate = DateTime.UtcNow.AddDays(1)
                },
                new TaskEntry
                {
                    Title = "Task 2",
                    Description = "Description 2",
                    Status = TaskEntryStatus.InProgress,
                    DueDate = DateTime.UtcNow.AddDays(2)
                }
            );

            await db.SaveChangesAsync();

            // Act
            var result = await TaskHandler.GetHandle(db);

            // Assert
            var okResult = Assert.IsType<Ok<List<TaskDTO>>>(result);
            var task = okResult.Value;

            Assert.Equal(2, task!.Count);
            Assert.Equal("Task 1", task[0].Title);
            Assert.Equal("Task 2", task[1].Title);
        }

        [Fact]
        public async Task GetByIdHandle_ExistingTask_ReturnsTask()
        {
            // Arrange
            using var database = new TestDatabase();
            var db = database.Context;

            var task = new TaskEntry
            {
                Title = "Task 1",
                Description = "Description 1",
                Status = TaskEntryStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            // Act
            var result = await TaskHandler.GetByIdHandle(task.Id, db);

            // Assert
            var okResult = Assert.IsType<Ok<TaskDTO>>(result);
            var taskDto = okResult.Value;

            Assert.Equal("Task 1", taskDto!.Title);
            Assert.Equal("Description 1", taskDto.Description);
        }

        [Fact]
        public async Task GetByIdHandle_NonExistingTask_ReturnsNotFound()
        {
            // Arrange
            using var database = new TestDatabase();
            var db = database.Context;

            // Act
            var result = await TaskHandler.GetByIdHandle(999, db);

            // Assert
            Assert.IsType<NotFound>(result);
        }

        [Fact]
        public async Task CreateHandle_ValidTask_ReturnsCreatedTask()
        {
            // Arrange
            using var database = new TestDatabase();
            var db = database.Context;

            var newTask = new CreateTaskDTO
            {
                Title = "New Task",
                Description = "New Description",
                Status = "Pending",
                DueDate = DateTime.UtcNow.AddDays(3)
            };

            // Act
            var result = await TaskHandler.CreateHandle(newTask, db);

            // Assert
            var createdResult = Assert.IsType<Created<TaskEntry>>(result);
            var task = createdResult.Value;

            Assert.NotNull(task);
            Assert.Equal("New Task", task!.Title);
            Assert.Equal("New Description", task.Description);
            Assert.Equal(1, await db.Tasks.CountAsync());
        }

        [Fact]
        public async Task CreateHandle_InvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            using var database = new TestDatabase();
            var db = database.Context;

            var newTask = new CreateTaskDTO
            {
                Title = "New Task",
                Description = "New Description",
                Status = "InvalidStatus",
                DueDate = DateTime.UtcNow.AddDays(3)
            };

            // Act
            var result = await TaskHandler.CreateHandle(newTask, db);

            // Assert
            Assert.IsType<BadRequest<string>>(result);
            Assert.Equal(0, await db.Tasks.CountAsync());
        }

        [Fact]
        public async Task CreateHandle_NullTask_ReturnsBadRequest()
        {
            // Arrange
            using var database = new TestDatabase();

            // Act
            var result = await TaskHandler.CreateHandle(
                null!,
                database.Context);

            // Assert
            Assert.IsType<BadRequest>(result);
        }

        [Fact]
        public async Task UpdateHandle_ExistingTask_UpdatesTitle()
        {
            // Arrage
            using var database = new TestDatabase();
            var db = database.Context;

            var task = new TaskEntry
            {
                Title = "Old Title",
                Description = "Old Description",
                Status = TaskEntryStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            var updateTask = new UpdateTaskDTO
            {
                Title = "New Title"
            };

            // Act
            var result = await TaskHandler.UpdateHandle(task.Id, updateTask, db);

            // Assert
            var okResult = Assert.IsType<Ok<TaskEntry>>(result);
            var updatedTask = okResult.Value;

            Assert.Equal("New Title", updatedTask!.Title);
            Assert.Equal("Old Description", updatedTask.Description);
        }

        [Fact]
        public async Task UpdateHandle_NonExistingTaskHandle_ReturnsNotFound()
        {
            // Arrange
            using var database = new TestDatabase();
            var db = database.Context;

            var updateTask = new UpdateTaskDTO
            {
                Title = "New Title"
            };

            // Act
            var result = await TaskHandler.UpdateHandle(999, updateTask, db);

            // Assert
            Assert.IsType<NotFound>(result);
        }

        [Fact]
        public async Task DeleteHandle_ExistingTask_DeletesTask()
        {
            // Arrange
            using var database = new TestDatabase();
            var db = database.Context;

            var task = new TaskEntry
            {
                Title = "Task to Delete",
                Description = "Description",
                Status = TaskEntryStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            // Act
            var result = await TaskHandler.DeleteHandle(task.Id, db);

            // Assert
            Assert.IsType<NoContent>(result);
            Assert.Equal(0, await db.Tasks.CountAsync());
        }

        [Fact]
        public async Task DeleteHandle_NonExistingTask_ReturnsNotFound()
        {
            // Arrange
            using var database = new TestDatabase();
            var db = database.Context;

            // Act
            var result = await TaskHandler.DeleteHandle(999, db);

            // Assert
            Assert.IsType<NotFound>(result);
        }
    }
}