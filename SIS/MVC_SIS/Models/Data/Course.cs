using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIS.Models.Data 
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
    }
}