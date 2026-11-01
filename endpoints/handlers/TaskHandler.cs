using Microsoft.EntityFrameworkCore;
using hmcts_test.model;

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
    }
}
