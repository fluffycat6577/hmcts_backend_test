using hmcts_test.model;
using System.Text.Json.Serialization;
namespace hmcts_test.controller
{
    public record TaskDTO
    {
        public required string Title { get; init; }
        public string? Description { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required model.TaskStatus Status { get; init; }
        public required DateTime DueDate { get; init; }
    }

    public record UpdateTaskDTO
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public model.TaskStatus? Status { get; init; }
        public DateTime? DueDate { get; init; }
    }
}
