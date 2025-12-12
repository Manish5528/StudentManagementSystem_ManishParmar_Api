using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Application.DTOs
{
    public class CreateCourseDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        [StringLength(100)]
        public string? Description { get; set; }
    }
}
