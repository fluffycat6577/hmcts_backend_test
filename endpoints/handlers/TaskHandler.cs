using Microsoft.EntityFrameworkCore;
using hmcts_test.model;
using hmcts_test.controller;

namespace hmcts_test.endpoints.handlers
{
    public static class TaskHandler
    {
        public static async Task<IResult> GetHandle(TaskContext db)
        {
            var tasks = await db.Tasks.ToListAsync();
            return Results.Ok(tasks);
        }

        public static async Task<IResult> GetByIdHandle(int id, TaskContext db)
        {
            var task = await db.Tasks.FindAsync(id);
            return task is not null ? Results.Ok(task) : Results.NotFound();
        }

        public static async Task<IResult> CreateHandle(TaskDTO task, TaskContext db)
        {
            var newTask = new model.Task
            {
                Title = task.Title,
                Description = task.Description,
                Status = (model.TaskStatus) task.Status,
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
            existingTask.Status = task.Status.HasValue ? (model.TaskStatus)task.Status.Value : existingTask.Status;
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
