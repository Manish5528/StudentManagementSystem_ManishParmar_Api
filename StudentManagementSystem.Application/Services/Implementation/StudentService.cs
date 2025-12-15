using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Application.Contract;
using StudentManagementSystem.Application.DTOs;
using StudentManagementSystem.Application.Services.Interface;
using StudentManagementSystem.Domain.Entities;
using System.Linq.Expressions;
 
namespace StudentManagementSystem.Application.Services.Implementation
{
    public class StudentService(IGenericRepository<Student> studentRepo,
                          IGenericRepository<Course> courseRepo,
                          IMapper mapper) : IStudentService
    {
        private readonly IGenericRepository<Student> _studentRepo = studentRepo;
        private readonly IGenericRepository<Course> _courseRepo = courseRepo;
        private readonly IMapper _mapper = mapper;
 
        public async Task<StudentDto> CreateAsync(CreateStudentDto dto)
        {
            if (await _studentRepo.ExistsAsync(s => EF.Property<string>(s, "PhoneNumber") == dto.PhoneNumber))
                throw new InvalidOperationException("Phone number already exists.");
 
            if (await _studentRepo.ExistsAsync(s => EF.Property<string>(s, "EmailId") == dto.EmailId))
                throw new InvalidOperationException("EmailId already exists.");
 
            var courseIds = dto.CourseIds?.Distinct().ToList() ?? [];
            foreach (var id in courseIds)
            {
                var c = await _courseRepo.GetByIdAsync(id);
                if (c == null) throw new KeyNotFoundException($"Course {id} not found.");
            }
 
            var student = _mapper.Map<Student>(dto);
 
            foreach (var id in courseIds)
            {
                student.StudentCourses.Add(new StudentCourse { StudentId = student.Id, CourseId = id });
            }
 
            await _studentRepo.AddAsync(student);
            return _mapper.Map<StudentDto>(student);
        }
 
        public async Task<bool> DeleteAsync(Guid id)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            if (student == null) return false;
 
            await _studentRepo.RemoveAsync(student);
            return true;
        }
 
        public async Task<StudentDto?> GetByIdAsync(Guid id)
        {
            Func<IQueryable<Student>, IQueryable<Student>> include = q =>
                q.Include(s => s.StudentCourses).ThenInclude(sc => sc.Course);
 
            var student = await _studentRepo.FirstOrDefaultAsync(s => s.Id == id, include: include);
            return student == null ? null : _mapper.Map<StudentDto>(student);
        }
 
        public async Task<(IEnumerable<StudentDto> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, bool asc)
        {
            Expression<Func<Student, bool>>? pred = null;
            if (!string.IsNullOrWhiteSpace(search))
                pred = s => s.FirstName.Contains(search) || s.LastName.Contains(search) || s.EmailId.Contains(search) || s.PhoneNumber.Contains(search);
 
            Func<IQueryable<Student>, IQueryable<Student>> include = q => q.Include(s => s.StudentCourses).ThenInclude(sc => sc.Course);
 
            Func<IQueryable<Student>, IOrderedQueryable<Student>>? orderBy = null;
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                orderBy = asc
                    ? (q => q.OrderBy(s => EF.Property<object>(s, sortBy)))
                    : (q => q.OrderByDescending(s => EF.Property<object>(s, sortBy)));
            }
            else orderBy = q => q.OrderBy(s => s.FirstName);
 
            var (items, total) = await _studentRepo.GetPagedAsync(pred, page, pageSize, orderBy, include);
            return (items.Select(i => _mapper.Map<StudentDto>(i)), total);
        }
 
        public async Task<StudentDto?> UpdateAsync(Guid id, UpdateStudentDto dto)
        {
            Func<IQueryable<Student>, IQueryable<Student>> include = q => q.Include(s => s.StudentCourses);
 
            var student = await _studentRepo.FirstOrDefaultAsync(s => s.Id == id, include: include);
            if (student == null) return null;
 
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && dto.PhoneNumber != student.PhoneNumber)
            {
                if (await _studentRepo.ExistsAsync(s => EF.Property<string>(s, "PhoneNumber") == dto.PhoneNumber))
                    throw new InvalidOperationException("Phone number already exists.");
            }
 
            if (!string.IsNullOrWhiteSpace(dto.EmailId) && !dto.EmailId.Equals(student.EmailId, StringComparison.CurrentCultureIgnoreCase))
            {
                if (await _studentRepo.ExistsAsync(s => EF.Property<string>(s, "EmailId") == dto.EmailId.ToLower()))
                    throw new InvalidOperationException("EmailId already exists.");
            }
 
            if (!string.IsNullOrWhiteSpace(dto.FirstName)) student.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName)) student.LastName = dto.LastName;
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) student.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(dto.EmailId)) student.EmailId = dto.EmailId.ToLower();
 
            if (dto.CourseIds != null)
            {
                var newIds = dto.CourseIds.Distinct().ToList();
                foreach (var cid in newIds)
                {
                    var c = await _courseRepo.GetByIdAsync(cid);
                    if (c == null) throw new KeyNotFoundException($"Course {cid} not found.");
                }
 
                var toRemove = student.StudentCourses.Where(sc => !newIds.Contains(sc.CourseId)).ToList();
                foreach (var r in toRemove) student.StudentCourses.Remove(r);
 
                var existing = student.StudentCourses.Select(sc => sc.CourseId).ToHashSet();
                foreach (var cid in newIds)
                {
                    if (!existing.Contains(cid))
                        student.StudentCourses.Add(new StudentCourse { StudentId = student.Id, CourseId = cid });
                }
            }
 
            await _studentRepo.UpdateAsync(student);
            return _mapper.Map<StudentDto>(student);
        }
    }
}