using Xunit;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using hmcts_test.model;
using hmctsBackend.Tests.Helpers;
using Task = System.Threading.Tasks.Task;
using TaskEntry = hmcts_test.model.Task;
using TaskEntryStatus = hmcts_test.model.TaskStatus;
using hmcts_test.controller;

namespace hmctsBackend.Tests.Integration
{
    public class TaskEndpointTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public TaskEndpointTests()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
            _factory.InitialiseDatabaseAsync().GetAwaiter().GetResult();
        }

        [Fact]
        public async Task GetAllTasks_ReturnsOk()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TaskContext>();

            db.Tasks.Add(new TaskEntry
            {
                Title = "Integration Test Task",
                Description = "This is a test task for integration testing.",
                Status = TaskEntryStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(3)
            });

            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/task");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var tasks = await response.Content.ReadFromJsonAsync<List<TaskDTO>>();
            Assert.NotNull(tasks);
            Assert.Single(tasks);
            Assert.Equal("Integration Test Task", tasks![0].Title);
        }

        [Fact]
        public async Task CreateTask_ReturnsCreated()
        {
            // Arrange
            var newTask = new CreateTaskDTO
            {
                Title = "New Integration Test Task",
                Description = "This is a new test task for integration testing.",
                Status = "Pending",
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/task", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdTask = await response.Content.ReadFromJsonAsync<TaskEntry>();
            Assert.NotNull(createdTask);
            Assert.Equal(newTask.Title, createdTask!.Title);

            // Verify that the task was actually saved in the database
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TaskContext>();
            var savedTask = await db.Tasks.FindAsync(createdTask.Id);
            Assert.NotNull(savedTask);
            Assert.Equal(newTask.Title, savedTask!.Title);
        }

        [Fact]
        public async Task UpdateTask_ExistingTask_ReturnsOk()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TaskContext>();

            var task = new TaskEntry
            {
                Title = "Task to Update",
                Description = "This task will be updated.",
                Status = TaskEntryStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            var updateTaskDto = new UpdateTaskDTO
            {
                Title = "Updated Task Title",
                Description = "Updated Description",
                Status = "InProgress",
                DueDate = DateTime.UtcNow.AddDays(10)
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/task/{task.Id}", updateTaskDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            using var scope2 = _factory.Services.CreateScope();
            var db2 = scope2.ServiceProvider.GetRequiredService<TaskContext>();
            var updatedTask = await db2.Tasks.FindAsync(task.Id);

            Assert.NotNull(updatedTask);
            Assert.Equal("Updated Task Title", updatedTask!.Title);
            Assert.Equal("Updated Description", updatedTask.Description);
        }

        [Fact]
        public async Task DeleteTask_ExistingTask_ReturnsNoContent()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TaskContext>();

            var task = new TaskEntry
            {
                Title = "Task to Delete",
                Description = "This task will be deleted.",
                Status = TaskEntryStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            // Act
            var response = await _client.DeleteAsync($"/task/{task.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var scope2 = _factory.Services.CreateScope();
            var db2 = scope2.ServiceProvider.GetRequiredService<TaskContext>();

            // Verify that the task was actually deleted from the database
            var deletedTask = await db2.Tasks.FindAsync(task.Id);
            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task GetTask_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingId = 999;

            // Act
            var response = await _client.GetAsync($"/task/{nonExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateTask_InvalidStatus_ReturnsBadRequest()
        {   
            // Arrange
            var newTask = new CreateTaskDTO
            {
                Title = "Invalid Status Task",
                Description = "This task has an invalid status.",
                Status = "InvalidStatus",
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/task", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTask_InvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TaskContext>();

            var task = new TaskEntry
            {
                Title = "Task to Update",
                Description = "This task will be updated.",
                Status = TaskEntryStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            var updateTaskDto = new UpdateTaskDTO
            {
                Title = "Updated Task Title",
                Description = "Updated Description",
                Status = "InvalidStatus",
                DueDate = DateTime.UtcNow.AddDays(10)
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/task/{task.Id}", updateTaskDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTask_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingId = 999;

            // Act
            var response = await _client.DeleteAsync($"/task/{nonExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

    }
}
