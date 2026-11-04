using Microsoft.EntityFrameworkCore;
using hmcts_test.model;
using hmcts_test.controller;

namespace hmcts_test.endpoints.handlers
{
    public static class TaskHandler
    {
        public static async Task<IResult> GetHandle(TaskContext db)
        {
            var tasks = await db.Tasks
                .Select(t => new TaskDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    DueDate = t.DueDate
                })
                .ToListAsync();

            return Results.Ok(tasks);
        }

        public static async Task<IResult> GetByIdHandle(int id, TaskContext db)
        {
            var task = await db.Tasks.FindAsync(id);
            var taskDTO = task is not null ? new TaskDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                DueDate = task.DueDate
            } : null;
            return taskDTO is not null ? Results.Ok(taskDTO) : Results.NotFound();
        }

        public static async Task<IResult> CreateHandle(CreateTaskDTO task, TaskContext db)
        {
            if (task is null)
            {
                return Results.BadRequest();
            }

            if (!Enum.TryParse<model.TaskStatus>(task.Status, out var status))
            {
                return Results.BadRequest("Invalid task status.");
            }

            if (!Enum.IsDefined(typeof(model.TaskStatus), status))
            {
                return Results.BadRequest("Invalid task status.");
            }

            var newTask = new model.Task
            {
                Title = task.Title,
                Description = task.Description,
                Status = status,
                DueDate = task.DueDate
            };

            db.Tasks.Add(newTask);
            await db.SaveChangesAsync();
            return Results.Created($"/Task/{newTask.Id}", newTask);
        }

        public static async Task<IResult> UpdateHandle(int id, UpdateTaskDTO task, TaskContext db)
        {
            var existingTask = await db.Tasks.FindAsync(id);
            if (existingTask is null)
            {
                return Results.NotFound();
            }
            existingTask.Title = task.Title ?? existingTask.Title;
            existingTask.Description = task.Description ?? existingTask.Description;

            if (task.Status is not null)
            {
                if (!Enum.TryParse<model.TaskStatus>(task.Status, out var status))
                {
                    return Results.BadRequest("Invalid task status.");
                }
                if (!Enum.IsDefined(typeof(model.TaskStatus), status))
                {
                    return Results.BadRequest("Invalid task status.");
                }
                existingTask.Status = status;
            }
            else
            {
                existingTask.Status = existingTask.Status;
            }

            existingTask.DueDate = task.DueDate ?? existingTask.DueDate;
            await db.SaveChangesAsync();
            return Results.Ok(existingTask);
        }

        public static async Task<IResult> DeleteHandle(int id, TaskContext db)
        {
            var existingTask = await db.Tasks.FindAsync(id);
            if (existingTask is null)
            {
                return Results.NotFound();
            }
            db.Tasks.Remove(existingTask);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }
    }
}
