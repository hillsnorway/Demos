using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DVDLibWebAPI.Data.Models;

namespace DVDLibWebAPI.Data
{
    public class DvdRepositoryEF : IDvdRepository
    {
        public DVDdb Add(DVDdb dvd)
        {
            using (var context = new DvdLibContextModel())
            {
                var idParam = new SqlParameter
                    { ParameterName = "@DvdID", SqlDbType = SqlDbType.Int, Value = 0, Direction = ParameterDirection.Output };
                var titleParam = new SqlParameter
                    { ParameterName = "@DvdTitle", SqlDbType = SqlDbType.VarChar, Value = dvd.title };
                var yearParam = new SqlParameter
                    { ParameterName = "@YearNR", SqlDbType = SqlDbType.VarChar, Value = dvd.realeaseYear };
                var directorParam = new SqlParameter
                    { ParameterName = "@DirectorName", SqlDbType = SqlDbType.VarChar, Value = dvd.director };
                var ratingParam = new SqlParameter
                    { ParameterName = "@RatingType", SqlDbType = SqlDbType.VarChar, Value = dvd.rating };
                var notesParam = new SqlParameter
                    { ParameterName = "@DvdNotes", SqlDbType = SqlDbType.VarChar, Value = dvd.notes };
                context.Database.SqlQuery<DVDdb>("AddDvd  @DvdID OUTPUT, @DvdTitle, @YearNR, @DirectorName, @RatingType, @DvdNotes", idParam, titleParam, yearParam, directorParam, ratingParam, notesParam).SingleOrDefault();
                dvd.dvdId = (int)idParam.Value;
            }
            return dvd;
        }

        public DVDdb Delete(int ID)
        {
            DVDdb deletedDvd = this.Get(ID);
            using (var context = new DvdLibContextModel())
            {
                var idParam = new SqlParameter
                    { ParameterName = "@DvdID", SqlDbType = SqlDbType.Int, Value = ID };
                context.Database.SqlQuery<DVDdb>("DeleteDvd @DvdID", idParam).SingleOrDefault();
            }
            return deletedDvd;
        }

        public DVDdb Edit(DVDdb dvd, int ID)
        {
            dvd.dvdId = ID; //Just in case the wrong ID was set...
            using (var context = new DvdLibContextModel())
            {
                var idParam = new SqlParameter
                    { ParameterName = "@DvdID", SqlDbType = SqlDbType.Int, Value = ID };
                var titleParam = new SqlParameter
                    { ParameterName = "@DvdTitle", SqlDbType = SqlDbType.VarChar, Value = dvd.title };
                var yearParam = new SqlParameter
                    { ParameterName = "@YearNR", SqlDbType = SqlDbType.VarChar, Value = dvd.realeaseYear };
                var directorParam = new SqlParameter
                    { ParameterName = "@DirectorName", SqlDbType = SqlDbType.VarChar, Value = dvd.director };
                var ratingParam = new SqlParameter
                    { ParameterName = "@RatingType", SqlDbType = SqlDbType.VarChar, Value = dvd.rating };
                var notesParam = new SqlParameter
                    { ParameterName = "@DvdNotes", SqlDbType = SqlDbType.VarChar, Value = dvd.notes };
                context.Database.SqlQuery<DVDdb>("EditDvd @DvdID, @DvdTitle, @YearNR, @DirectorName, @RatingType, @DvdNotes", idParam, titleParam, yearParam, directorParam, ratingParam, notesParam).SingleOrDefault();
            }
            return dvd;
        }

        public DVDdb Get(int ID)
        {
            DVDdb dvd = new DVDdb();
            using (var context = new DvdLibContextModel())
            {
                var idParam = new SqlParameter
                { ParameterName = "@DvdID", SqlDbType = SqlDbType.Int, Value = ID };
                dvd = context.Database.SqlQuery<DVDdb>("GetDvdByID @DvdID", idParam).Single();

            }
            return dvd;
        }

        public Dictionary<int, DVDdb> GetAll()
        {
            Dictionary<int, DVDdb> dictDVDs = new Dictionary<int, DVDdb>();
            using (var context = new DvdLibContextModel())
            {
                List<DVDdb> dvds = context.Database.SqlQuery<DVDdb>("GetAllDvds").ToList();
                foreach (DVDdb dvd in dvds)
                {
                    dictDVDs.Add(dvd.dvdId, dvd);
                }
            }
            return dictDVDs;// dictDVDs;
        }
    }
}
