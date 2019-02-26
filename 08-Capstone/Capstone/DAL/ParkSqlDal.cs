using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class ParkSqlDal
    {
        private string connectionString;
        private const string SQL_GetAllParks = @"Select * From park ORDER BY park.name;";

        public ParkSqlDal (string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public List<Park> GetAllParks()
        {
            List<Park> parks = new List<Park>();

            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL_GetAllParks, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Park park = new Park
                        {
                            Id = Convert.ToInt32(reader["park_id"]),
                            Name = Convert.ToString(reader["name"]),
                            Location = Convert.ToString(reader["location"]),
                            EstablishDate = Convert.ToDateTime(reader["establish_date"]),
                            Area = Convert.ToInt32(reader["area"]),
                            Visitors = Convert.ToInt32(reader["visitors"]),
                            Description = Convert.ToString(reader["description"])
                        };
                        parks.Add(park);
                    }
                }
            }
            catch
            {
                throw;
            }
            return parks;
        }
    }
}
