using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIS.Models.Data
{
    public class Student : IValidatableObject 
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal GPA { get; set; }
        public Address Address { get; set; }
        public Major Major { get; set; }
        public List<Course> Courses { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if (string.IsNullOrEmpty(FirstName))
            {
                errors.Add(new ValidationResult("The First Name field is required!", new[] { "FirstName" }));
            }

            if (string.IsNullOrEmpty(LastName))
            {
                errors.Add(new ValidationResult("The Last Name field is required!", new[] { "LastName" }));
            }

            if (GPA < 0 || GPA > 4)
            {
                errors.Add(new ValidationResult("A valid GPA (0 =< GPA <= 4) must be entered!", new[] { "GPA" }));
            }

            return errors;

        }
    }
}