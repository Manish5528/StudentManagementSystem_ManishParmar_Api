using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Application.DTOs;
using StudentManagementSystem.Application.Services.Interface;
 
namespace StudentManagementSystem.API.Controllers;
 
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentController(IStudentService studentService) : ControllerBase
{
    private readonly IStudentService _studentService = studentService;
 
    [HttpGet]
    public async Task<IActionResult> GetPageData(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        string? sortBy = "FirstName",
        bool asc = true)
    {
        (IEnumerable<StudentDto>? items, int total) = await _studentService.GetPagedAsync(page, pageSize, search, sortBy, asc);
 
        return Ok(new
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        });
    }
 
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        StudentDto? result = await _studentService.GetByIdAsync(id);
        if (result == null) return NotFound();
 
        return Ok(result);
    }
 
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
 
        try
        {
            StudentDto created = await _studentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
 
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
 
        try
        {
            StudentDto? result = await _studentService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
 
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
           return NotFound(new { message = ex.Message });
        }
    }
 
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        bool deleted = await _studentService.DeleteAsync(id);
        if (!deleted) return NotFound();
 
        return NoContent();
    }
}