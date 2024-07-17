using System.ComponentModel.DataAnnotations;

namespace To_Do_List.Models
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        public string Priority { get; set; } // Low, Medium, High

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        public string Category { get; set; } // Work, Personal, etc.
    }
}
