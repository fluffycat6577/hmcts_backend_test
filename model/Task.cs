using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hmcts_test.model
{
    [Table("Task")]
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public required string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public TaskStatus Status { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
    }

    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }
}
