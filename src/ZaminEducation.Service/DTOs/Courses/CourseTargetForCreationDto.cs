﻿using System.ComponentModel.DataAnnotations;

namespace ZaminEducation.Service.DTOs.Courses
{
    public class CourseTargetForCreationDto
    {
        [Required]
        public string Name { get; set; }
        public long CourseId { get; set; }
    }
}
