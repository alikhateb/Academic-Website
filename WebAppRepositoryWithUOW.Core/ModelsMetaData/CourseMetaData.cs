﻿using System.ComponentModel.DataAnnotations;

namespace WebAppRepositoryWithUOW.Core.ModelsMetaData
{
    public class CourseMetaData
    {
        [Required(ErrorMessage = "name is required"),
         MaxLength(length: 50, ErrorMessage = "name must be less than 50 character")]
        public string Name { get; set; }



        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
    }
}