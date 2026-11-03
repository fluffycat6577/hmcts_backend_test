using hmcts_test.model;
using System.Text.Json.Serialization;
namespace hmcts_test.controller
{
    public record TaskDTO
    {
        public int Id { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public required string Status { get; init; }
        public required DateTime DueDate { get; init; }
    }
    public record CreateTaskDTO
    {
        public required string Title { get; init; }
        public string? Description { get; init; }
        public required string Status { get; init; }
        public required DateTime DueDate { get; init; }
    }

    public record UpdateTaskDTO
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public string? Status { get; init; }
        public DateTime? DueDate { get; init; }
    }
}
