using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Application.DTOs
{
    public class CreateStudentDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{1,10}$", ErrorMessage = "PhoneNumber must be digits only (max 10).")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string EmailId { get; set; } = null!;
        public List<Guid> CourseIds { get; set; } = new();
    }
}
