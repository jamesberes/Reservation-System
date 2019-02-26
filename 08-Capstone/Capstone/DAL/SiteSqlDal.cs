using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class SiteSqlDal
    {
        private string connectionString;
        private const string SQL_GetAllSitesFromCampground = @"Select * From site where campground_id = @campgroundId;";

        //Replaced by the following statement. This is kept here to remember what we did wrong (not select all cases)
        /*private const string SQL_GetTop5AvailableSitesFromCampground = 
            "SELECT TOP 5 * FROM site " +
            "JOIN campground ON campground.campground_id = site.campground_id " +
            "WHERE site.site_id NOT IN" +
            "(SELECT reservation.site_id FROM reservation " +
            "WHERE(@userEnteredCheckInDate <= reservation.from_date AND @userEnteredCheckOutDate >= reservation.to_date) " +
            "OR(@userEnteredCheckInDate <= reservation.from_date AND @userEnteredCheckOutDate <= reservation.to_date AND @userEnteredCheckOutDate >= reservation.from_date) " +
            "OR (@userEnteredCheckInDate >= reservation.from_date AND @userEnteredCheckOutDate <= reservation.to_date) " +
            "OR (@userEnteredCheckInDate >= reservation.from_date AND @userEnteredCheckInDate <= reservation.from_date AND @userEnteredCheckOutDate >= reservation.to_date)) " +
            "AND campground.campground_id = @campgroundId;";
            */

        private const string SQL_GetTop5AvailableSitesFromCampground =
            "SELECT * " +
            "FROM site " +
            "WHERE site.campground_id = @campgroundId " +
            "AND site.site_id NOT IN " +
            "(SELECT site.site_id from reservation " +
            "JOIN site on reservation.site_id = site.site_id " +
            "WHERE site.campground_id = @campgroundId " +
            "AND (reservation.to_date > @userEnteredCheckInDate " +
            "AND reservation.from_date< @userEnteredCheckOutDate));";

        //TODO bonus ParkWide Search sql statement
        //private const string SQL_GetAvailableSitesFromPark =
        //    @"Select TOP 5 * " +
        //    "From reservation " +
        //    "JOIN site ON reservation.site_id = site.site_id " +
        //    "JOIN campground ON site.campground_id = campground.campground_id " +
        //    "JOIN park ON park.park_id = campground.park_id " +
        //    "where park.park_id = @parkId" +
        //    "AND @userEnteredDate NOT BETWEEN reservation.from_date AND reservation.to_date " +
        //    "AND @userEnteredCheckOutDate NOT BETWEEN reservation.from_date AND reservation.to_date;";

        //Constructor
        public SiteSqlDal(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        
        //Methods
        public List<Site> GetAllSitesFromCampground(int campgroundId)
        {
            List<Site> sitesFromSpecifiedCampground = new List<Site>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL_GetAllSitesFromCampground, conn);

                    //Parameters
                    cmd.Parameters.AddWithValue("@campgroundId", campgroundId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Site site = new Site
                        {
                            Id = Convert.ToInt32(reader["site_id"]),
                            CampgroundId = Convert.ToInt32(reader["campground_id"]),
                            SiteNumber = Convert.ToInt32(reader["site_number"]),
                            MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]),
                            Accessible = Convert.ToBoolean(reader["accessible"]),
                            MaxRv = Convert.ToInt32(reader["max_rv_length"]),
                            Utilities = Convert.ToBoolean(reader["utilities"])
                        };
                        sitesFromSpecifiedCampground.Add(site);
                    }
                }
            }
            catch
            {
                throw;
            }
            return sitesFromSpecifiedCampground;
        }
        
        public List<Site> GetAvailableSitesFromCampground(int campgroundId, DateTime checkInDate, DateTime checkOutDate)
        {
            List<Site> availableSitesFromCampground = new List<Site>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL_GetTop5AvailableSitesFromCampground, conn);

                    //Parameters
                    cmd.Parameters.AddWithValue("@campgroundId", campgroundId);
                    cmd.Parameters.AddWithValue("@userEnteredCheckInDate", checkInDate);
                    cmd.Parameters.AddWithValue("@userEnteredCheckOutDate", checkOutDate);
                    
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Site site = new Site
                        {
                            Id = Convert.ToInt32(reader["site_id"]),
                            CampgroundId = Convert.ToInt32(reader["campground_id"]),
                            SiteNumber = Convert.ToInt32(reader["site_number"]),
                            MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]),
                            Accessible = Convert.ToBoolean(reader["accessible"]),
                            MaxRv = Convert.ToInt32(reader["max_rv_length"]),
                            Utilities = Convert.ToBoolean(reader["utilities"])                            
                        };
                        availableSitesFromCampground.Add(site);
                    }
                }
            }
            catch
            {
                throw;
            }
            return availableSitesFromCampground;
        }

        //Todo : Bonus ParkWide Search Method
        //public List<Site> GetAvailableSitesFromPark(int parkId, DateTime checkInDate, DateTime checkOutDate)
        //{
        //    List<Site> availableSitesFromPark = new List<Site>();

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            SqlCommand cmd = new SqlCommand(SQL_GetTop5DistinctAvailableSitesFromPark, conn);

        //            //Parameters
        //            cmd.Parameters.AddWithValue("@parkId", parkId);
        //            cmd.Parameters.AddWithValue("@userEnteredCheckInDate", checkInDate);
        //            cmd.Parameters.AddWithValue("@userEnteredCheckOutDate", checkOutDate);

        //            SqlDataReader reader = cmd.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                Site site = new Site
        //                {
        //                    Id = Convert.ToInt32(reader["site_id"]),
        //                    CampgroundId = Convert.ToInt32(reader["campground_id"]),
        //                    SiteNumber = Convert.ToInt32(reader["site_number"]),
        //                    MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]),
        //                    Accessible = Convert.ToBoolean(reader["accessible"]),
        //                    MaxRv = Convert.ToInt32(reader["max_rv_length"]),
        //                    Utilities = Convert.ToBoolean(reader["utilities"])
        //                };
        //                availableSitesFromPark.Add(site);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return availableSitesFromPark;
        //}
    }
}
