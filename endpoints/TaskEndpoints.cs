using hmcts_test.endpoints.handlers;
using hmcts_test.controller;

namespace hmcts_test.endpoints
{
    public static class TaskEndpoints
    {
        public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/Task").WithTags("Task");

            group.MapGet("/", TaskHandler.GetHandle).WithName("GetAllTasks").Produces<List<model.Task>>(StatusCodes.Status200OK);
            group.MapGet("/{id}", TaskHandler.GetByIdHandle).WithName("GetTaskById").Produces<model.Task>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
            group.MapPost("/", TaskHandler.CreateHandle).WithName("CreateTask").Accepts<TaskDTO>("application/json").Produces<model.Task>(StatusCodes.Status201Created);
            group.MapPut("/{id}", TaskHandler.UpdateHandle).WithName("UpdateTask").Accepts<UpdateTaskDTO>("application/json").Produces<model.Task>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
            group.MapDelete("/{id}", TaskHandler.DeleteHandle).WithName("DeleteTask").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);

            return routes;
        }
    }
}
