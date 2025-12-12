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
            using var stream = new MemoryStream(fileBytes);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            });

            var records = new List<CreateCourseDto>();
            try
            {
                records = csv.GetRecords<CreateCourseDto>().ToList();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("CSV file contains invalid data.");
            }

            int count = 0;
            foreach (var dto in records)
            {
                bool exists = await _courseRepo.ExistsAsync(c => c.Name == dto.Name);
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
            var courses = await _courseRepo.GetAllAsync();
            return courses.Select(c => _mapper.Map<CourseDto>(c));
        }

    }
}
