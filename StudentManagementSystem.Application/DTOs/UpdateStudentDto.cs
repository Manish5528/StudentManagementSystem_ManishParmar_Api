using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Application.DTOs
{
    public class UpdateStudentDto
    {
        [StringLength(50, MinimumLength = 1)]
        public string? FirstName { get; set; }

        [StringLength(50, MinimumLength = 1)]
        public string? LastName { get; set; }

        [RegularExpression(@"^\d{1,10}$", ErrorMessage = "PhoneNumber must be digits only (max 10).")]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public string? EmailId { get; set; }

        public List<Guid>? CourseIds { get; set; }
    }
}
