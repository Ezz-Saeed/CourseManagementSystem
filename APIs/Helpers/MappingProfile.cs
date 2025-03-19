using APIs.DTOs.CourseDtos;
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
                .ForMember(d=>d.TrainerFirstName, opt=>opt.MapFrom(s=>s.Trainer.FirstName))
                .ForMember(d => d.TrainerLastName, opt => opt.MapFrom(s => s.Trainer.LastName))
                .ForMember(d => d.TrainerEmail, opt => opt.MapFrom(s => s.Trainer.Email));
        }
    }
}
