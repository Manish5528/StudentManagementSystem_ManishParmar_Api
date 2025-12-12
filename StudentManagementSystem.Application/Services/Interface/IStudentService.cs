using StudentManagementSystem.Application.DTOs;

namespace StudentManagementSystem.Application.Services.Interface
{
    public interface IStudentService
    {
        Task<(IEnumerable<StudentDto> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, bool asc);
        Task<StudentDto?> GetByIdAsync(Guid id);
        Task<StudentDto> CreateAsync(CreateStudentDto dto);
        Task<StudentDto?> UpdateAsync(Guid id, UpdateStudentDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}