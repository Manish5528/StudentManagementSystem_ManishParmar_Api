using AutoMapper;
using StudentManagementSystem.Application.DTOs;
using StudentManagementSystem.Domain.Entities;
using System.Linq;

namespace StudentManagementSystem.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.CourseIds, opt => opt.MapFrom(src => src.StudentCourses.Select(sc => sc.CourseId).ToList()));

            CreateMap<CreateStudentDto, Student>()
                .ForMember(dest => dest.StudentCourses, opt => opt.Ignore());

            CreateMap<UpdateStudentDto, Student>()
                .ForMember(dest => dest.StudentCourses, opt => opt.Ignore());

            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.StudentIds, opt => opt.MapFrom(src => src.StudentCourses.Select(sc => sc.StudentId).ToList()));

            CreateMap<CreateCourseDto, Course>();
        }
    }
}
