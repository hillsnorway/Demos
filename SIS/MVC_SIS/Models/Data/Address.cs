using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIS.Models.Data
{
    public class Address : IValidatableObject
    {
        public int AddressId { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public State State { get; set; }
        public string PostalCode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if (string.IsNullOrEmpty(Street1))
            {
                errors.Add(new ValidationResult("The Address1 field is required!", new[] { "Street1" }));
            }

            if (string.IsNullOrEmpty(City))
            {
                errors.Add(new ValidationResult("The City field is required!", new[] { "City" }));
            }

            if (State == null || string.IsNullOrEmpty(State.StateAbbreviation))
            {
                errors.Add(new ValidationResult("The State is required!", new[] { "State" }));
            }

            int intPostalCode;
            if (string.IsNullOrEmpty(PostalCode))
            {
                errors.Add(new ValidationResult("The Postal Code field is required!", new[] { "PostalCode" }));
            }
            else if (!PostalCode.All(c => Char.IsDigit(c)))
            {
                errors.Add(new ValidationResult("The Postal Code must consist of only digits!", new[] { "PostalCode" }));
            }
            else if (PostalCode.Length != 5)
            {
                errors.Add(new ValidationResult("The Postal Code must be 5 digits in length!", new[] { "PostalCode" }));
            }
            else if (int.TryParse(PostalCode, out intPostalCode))
            {
                if (intPostalCode < 501 || intPostalCode > 99950)
                    errors.Add(new ValidationResult("The Postal Code must be between 00501 & 99950!", new[] { "PostalCode" }));
            }

            return errors;
            

        }
    }
}