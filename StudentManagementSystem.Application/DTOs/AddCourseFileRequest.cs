using Microsoft.AspNetCore.Http;

namespace StudentManagementSystem.Application.DTOs
{
    public record FileRequest(IFormFile File);
}