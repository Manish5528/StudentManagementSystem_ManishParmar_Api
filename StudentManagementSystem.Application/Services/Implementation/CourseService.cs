using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using StudentManagementSystem.Application.Contract;
using StudentManagementSystem.Application.DTOs;
using StudentManagementSystem.Application.Services.Interface;
using StudentManagementSystem.Domain.Entities;
using System.Globalization;
 
namespace StudentManagementSystem.Application.Services.Implementation
{
    public class CourseService(IGenericRepository<Course> courseRepo, IMapper mapper) : ICourseService
    {
        private readonly IGenericRepository<Course> _courseRepo = courseRepo;
        private readonly IMapper _mapper = mapper;
 
        public async Task<int> BulkImportAsync(byte[] fileBytes)
        {
            using MemoryStream stream = new(fileBytes);
            using StreamReader reader = new(stream);
            using CsvReader csv = new(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            });
 
            List<CreateCourseDto> records = [];
            try
            {
                records = [.. csv.GetRecords<CreateCourseDto>()];
            }
            catch (Exception)
            {
                throw new InvalidOperationException("CSV file contains invalid data.");
            }
 
            int count = 0;
            foreach (CreateCourseDto dto in records)
            {
                bool exists = await _courseRepo.ExistsAsync(c => c.Name.ToLower() == dto.Name.ToLower());
                if (dto.Description?.Length > 100)
                    throw new InvalidOperationException("Description exceeds 100 characters.");
                if (string.IsNullOrWhiteSpace(dto.Name))
                    continue;
 
                if (!exists)
                {
                    var entity = _mapper.Map<Course>(dto);
                    await _courseRepo.AddAsync(entity);
                    count++;
                }
            }
 
            return count;
        }
 
        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            IEnumerable<Course> courses = await _courseRepo.GetAllAsync();
            return courses.Select(c => _mapper.Map<CourseDto>(c));
        }
 
    }
}