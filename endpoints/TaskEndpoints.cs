using hmcts_test.endpoints.handlers;

namespace hmcts_test.endpoints
{
    public static class TaskEndpoints
    {
        public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/Task").WithTags("Task");

            group.MapGet("/", TaskHandler.GetHandle).WithName("GetAllTasks").Produces<List<hmcts_test.model.Task>>(StatusCodes.Status200OK);
            group.MapGet("/{id}", TaskHandler.GetByIdHandle).WithName("GetTaskById").Produces<hmcts_test.model.Task>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

            return routes;
        }
    }
}
