﻿using System.ComponentModel.DataAnnotations;

namespace APIs.DTOs.CourseDtos
{
    public class UpdateCourseDto
    {
        [MaxLength(150)]
        public string? Name { get; set; }
        [MaxLength(150)]
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        //public string? TrainerUserName { get; set; }
    }
}
