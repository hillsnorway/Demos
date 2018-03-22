namespace DVDLibWebAPI.Data.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DvdLibContextModel : DbContext
    {
        public DvdLibContextModel()
            : base("name=DvdLibContextModel")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
