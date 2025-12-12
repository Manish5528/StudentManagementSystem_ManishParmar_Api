

namespace StudentManagementSystem.Application.DTOs;

public class StudentDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string EmailId { get; set; } = null!;
    public List<Guid> CourseIds { get; set; } = [];
}


