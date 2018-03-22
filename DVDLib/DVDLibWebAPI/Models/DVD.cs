using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DVDLibWebAPI.Models
{
    public class DVD
    {
        public int dvdId { get; set; }
        public string title { get; set; } //existed
        public string realeaseYear { get; set; }
        public string director { get; set; }
        public string rating { get; set; } //existed
        public string notes { get; set; }
    }
}