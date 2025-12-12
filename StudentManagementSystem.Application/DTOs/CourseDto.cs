using System;
using System.Collections.Generic;

namespace StudentManagementSystem.Application.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<Guid> StudentIds { get; set; } = new();
    }
}
