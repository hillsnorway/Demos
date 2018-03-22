using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using DVDLibWebAPI.Data.Models;
using System.Configuration;

namespace DVDLibWebAPI.Data
{
    public class DvdRepositoryADO : IDvdRepository
    {
        public DVDdb Add(DVDdb dvd)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["DvdDB"].ConnectionString;

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AddDvd";

                // Create parameter with Direction as Output (and correct name and type)
                SqlParameter outputDvdIDParam = new SqlParameter("@DvdID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputDvdIDParam);
                // Add other input parameters
                cmd.Parameters.AddWithValue("@DvdTitle", dvd.title);
                cmd.Parameters.AddWithValue("@YearNR", dvd.realeaseYear);
                cmd.Parameters.AddWithValue("@DirectorName", dvd.director);
                cmd.Parameters.AddWithValue("@RatingType", dvd.rating);
                cmd.Parameters.AddWithValue("@DvdNotes", dvd.notes);

                //Open the connection and run the SP
                conn.Open();
                cmd.ExecuteNonQuery();
                //Retrieve output parameter value
                dvd.dvdId = (int)outputDvdIDParam.Value;
            }
            return dvd;
        }

        public DVDdb Delete(int ID)
        {
            DVDdb deletedDvd = this.Get(ID);

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["DvdDB"].ConnectionString;

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DeleteDvd";

                // Add input parameters
                cmd.Parameters.AddWithValue("@DvdID", ID);

                //Open the connection and run the SP
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return deletedDvd;
        }

        public DVDdb Edit(DVDdb dvd, int ID)
        {
            dvd.dvdId = ID; //Just incase the ID was not set in the dvd object

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["DvdDB"].ConnectionString;

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "EditDvd";

                // Add input parameters
                cmd.Parameters.AddWithValue("@DvdID", ID);
                cmd.Parameters.AddWithValue("@DvdTitle", dvd.title);
                cmd.Parameters.AddWithValue("@YearNR", dvd.realeaseYear);
                cmd.Parameters.AddWithValue("@DirectorName", dvd.director);
                cmd.Parameters.AddWithValue("@RatingType", dvd.rating);
                cmd.Parameters.AddWithValue("@DvdNotes", dvd.notes);

                //Open the connection and run the SP
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return dvd;
        }

        public DVDdb Get(int ID)
        {
            DVDdb dvd = new DVDdb();

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["DvdDB"].ConnectionString;

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetDvdByID";

                // Add input parameters
                cmd.Parameters.AddWithValue("@DvdID", ID);

                //Open the connection
                conn.Open();
                using(SqlDataReader dr = cmd.ExecuteReader())
                {
                    //Result set is a single row
                    dr.Read();
                    //Populate the return object
                    dvd.dvdId = (int)dr["dvdId"];
                    dvd.title = dr["title"].ToString();
                    dvd.realeaseYear = dr["realeaseYear"].ToString();
                    dvd.director = dr["director"].ToString();
                    dvd.rating = dr["rating"].ToString();
                    dvd.notes = dr["notes"].ToString();
                }
            }
            return dvd;
        }

        public Dictionary<int, DVDdb> GetAll()
        {

            Dictionary<int, DVDdb> dvds = new Dictionary<int, DVDdb>();

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["DvdDB"].ConnectionString;

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetAllDvds";

                //Open the connection
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        DVDdb dvd = new DVDdb();
                        //Populate the dvd object
                        dvd.dvdId = (int)dr["dvdId"];
                        dvd.title = dr["title"].ToString();
                        dvd.realeaseYear = dr["realeaseYear"].ToString();
                        dvd.director = dr["director"].ToString();
                        dvd.rating = dr["rating"].ToString();
                        dvd.notes = dr["notes"].ToString();
                        //Add to dictionary return object
                        dvds.Add(dvd.dvdId, dvd);
                    }
                }
            }
            return dvds;
        }
    }
}
