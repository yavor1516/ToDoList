using System.ComponentModel.DataAnnotations;

namespace To_Do_List.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; } // Primary key

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>(); // Navigation property
    }
}
