using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class CampgroundSqlDal
    {
        private string connectionString;
        private const string SQL_GetAllCampgroundsFromPark = @"Select * From campground where park_id = @parkId;";

        public CampgroundSqlDal(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public List<Campground> GetAllCampgroundsFromPark(int parkId)
        {
            List<Campground> campgroundsFromSpecifiedPark = new List<Campground>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL_GetAllCampgroundsFromPark, conn);

                    //Parameters
                    cmd.Parameters.AddWithValue("@parkId", parkId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Campground campground = new Campground
                        {
                            Id = Convert.ToInt32(reader["campground_id"]),
                            ParkId = Convert.ToInt32(reader["park_id"]),
                            Name = Convert.ToString(reader["name"]),
                            MonthOpenFrom = Convert.ToInt32(reader["open_from_mm"]),
                            MonthOpenTo = Convert.ToInt32(reader["open_to_mm"]),
                            DailyFee = Convert.ToDecimal(reader["daily_fee"])
                        };
                        campgroundsFromSpecifiedPark.Add(campground);
                    }
                }
            }
            catch
            {
                throw;
            }
            return campgroundsFromSpecifiedPark;
        }
    }
}
