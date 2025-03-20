using APIs.DTOs.CourseDtos;
using APIs.DTOs.TrainerDtos;
using APIs.Models;
using AutoMapper;

namespace APIs.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddCourseDto, Course>();

            CreateMap<Course, GetCourseDto>()
                .ForMember(d=>d.TrainerFirstName, opt=>opt.MapFrom(s=>s.Trainer!.FirstName))
                .ForMember(d => d.TrainerLastName, opt => opt.MapFrom(s => s.Trainer!.LastName))
                .ForMember(d => d.TrainerEmail, opt => opt.MapFrom(s => s.Trainer!.Email))
                .ForMember(d => d.TrainerUserName, opt => opt.MapFrom(s => s.Trainer!.UserName));

            CreateMap<Course, CourseDto>();

            CreateMap<Appuser, GetTrainerDto>()
                .ForMember(d=>d.Courses, opt=>opt.MapFrom(s=>s.Courses));
        }
    }
}
