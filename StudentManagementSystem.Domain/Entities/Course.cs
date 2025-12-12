 
 namespace StudentManagementSystem.Domain.Entities;
 public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; } = [];
    }