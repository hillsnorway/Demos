using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIS.Models.Data
{
    public class Major
    {
        [DisplayName("Major")]
        public int MajorId { get; set; }
        public string MajorName { get; set; }
    }
}