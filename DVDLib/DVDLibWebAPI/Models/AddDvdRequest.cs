using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DVDLibWebAPI.Models
{
    public class AddDvdRequest
    {
        [Required]
        public string title { get; set; }
        [Required]
        public string realeaseYear { get; set; }
        [Required]
        public string director { get; set; }
        [Required]
        public string rating { get; set; }
        public string notes { get; set; }

    }
}