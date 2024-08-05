using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace To_Do_List.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        [Required]
        public string Priority { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string UserEmail { get; set; } // The email of the user who owns the task

        [Required]
        public string Progress { get; set; } // Not Started, In Progress, Completed

        public DateTime? ReminderTime { get; set; } // Reminder time

        [ForeignKey("User")]
        public int UserId { get; set; } // Foreign key to User table
        public User User { get; set; }  // Navigation property
    }
}
