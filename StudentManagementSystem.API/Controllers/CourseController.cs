using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Application.DTOs;
using StudentManagementSystem.Application.Services.Interface;
 
namespace StudentManagementSystem.API.Controllers;
 
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CourseController(ICourseService courseService) : ControllerBase
{
    private readonly ICourseService _courseService = courseService;
 
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> BulkImport([FromForm] FileRequest request)
    {
        IFormFile file = request.File;
        if (file == null)
            return BadRequest(new { message = "File is required." });
 
        if (file.Length == 0)
            return BadRequest(new { message = "Uploaded file is empty." });
 
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "File size must not exceed 5 MB." });
 
        if (!file.FileName.EndsWith(".csv"))
            return BadRequest(new { message = "Only CSV files are allowed." });
 
 
        byte[] fileBytes;
 
        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            fileBytes = stream.ToArray();
        }
 
        try
        {
            var importedCount = await _courseService.BulkImportAsync(fileBytes);
            return Ok(new { imported = importedCount });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
 
 
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _courseService.GetAllAsync());
    }
}