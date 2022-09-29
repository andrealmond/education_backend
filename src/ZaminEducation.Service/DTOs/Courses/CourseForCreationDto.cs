﻿using System.ComponentModel.DataAnnotations;
using ZaminEducation.Domain.Enums;

namespace ZaminEducation.Service.DTOs.Courses
{
    public class CourseForCreationDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public long AuthorId { get; set; }

        public long ImageId { get; set; }

        public long CategoryId { get; set; }

        [Required]
        public CourseLevel Level { get; set; }
    }
}