using StudentManagementSystem.Application.DTOs;

namespace StudentManagementSystem.Application.Services.Interface;

public interface ICourseService
{
    Task<int> BulkImportAsync(byte[] fileBytes);

    Task<IEnumerable<CourseDto>> GetAllAsync();
}
