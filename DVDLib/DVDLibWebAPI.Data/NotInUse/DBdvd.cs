using System;
using System.Collections.Generic;
using System.Text;

namespace DVDLibWebAPI.Data.Models
{
    public class DBdvd
    {
        public int DvdID { get; set; }
        public DByear DvdYear { get; set; }
        public DBdirector DvdDirector { get; set; }
        public DBrating DvdRating { get; set; }
        public string DvdTitle { get; set; }
        public string DvdNotes { get; set; }
    }
}
