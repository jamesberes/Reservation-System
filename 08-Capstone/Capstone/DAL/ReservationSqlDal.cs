using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ReservationSqlDal
    {
        private string connectionString;
        private const string SQL_GetAllReservations = @"Select * from reservation;";
        private const string SQL_GetAllReservationsFromCampground = @"Select * From reservation  JOIN site ON reservation.site_id = site.site_id " +
            "JOIN campground ON campground.campground_id = site.campground_id WHERE campground.campground_id = @campgroundId;";
        private const string SQL_MakeReservation = @"INSERT INTO reservation (site_id, name, from_date, to_date) " +
                        "VALUES (@site_id, @name, @from_date, @to_date); SELECT CAST(SCOPE_IDENTITY() as int);";

        public ReservationSqlDal(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public List<Reservation> GetAllReservations()
        {
            List<Reservation> reservations = new List<Reservation>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL_GetAllReservations, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Reservation reservation = new Reservation
                        {
                            Id = Convert.ToInt32(reader["reservation_id"]),
                            SiteId = Convert.ToInt32(reader["site_id"]),
                            Name = Convert.ToString(reader["name"]),
                            FromDate = Convert.ToDateTime(reader["from_date"]),
                            ToDate = Convert.ToDateTime(reader["to_date"]),
                            CreateDate = Convert.ToDateTime(reader["create_date"])
                        };
                        reservations.Add(reservation);
                    }
                }
            }
            catch
            {
                throw;
            }
            return reservations;
        }

        public List<Reservation> GetAllReservationsFromCampground(int campgroundId)
        {
            List<Reservation> reservations = new List<Reservation>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL_GetAllReservationsFromCampground, conn);
                    cmd.Parameters.AddWithValue("@campgroundId", campgroundId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Reservation reservation = new Reservation
                        {
                            Id = Convert.ToInt32(reader["reservation_id"]),
                            SiteId = Convert.ToInt32(reader["site_id"]),
                            Name = Convert.ToString(reader["name"]),
                            FromDate = Convert.ToDateTime(reader["from_date"]),
                            ToDate = Convert.ToDateTime(reader["to_date"]),
                            CreateDate = Convert.ToDateTime(reader["create_date"])
                        };
                        reservations.Add(reservation);
                    }
                }
            }
            catch
            {
                throw;
            }
            return reservations;
        }
        
        public int MakeReservation(int siteId, string name, DateTime checkInDate, DateTime checkOutDate)
        {
            int reservationId = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    //Insert some reservations
                    cmd = new SqlCommand(SQL_MakeReservation, connection);
                    cmd.Parameters.AddWithValue("@site_id", siteId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@from_date", checkInDate);
                    cmd.Parameters.AddWithValue("@to_date", checkOutDate);

                    reservationId = (int)cmd.ExecuteScalar();
                }
            }
            catch
            {
                throw;
            }
            return reservationId;
        }        
    }
}
