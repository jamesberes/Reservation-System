using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Transactions;
using System.Data.SqlClient;
using Capstone.Models;
using Capstone.DAL;
using System;
using Capstone;

namespace Capstone.Tests.DALTests
{
    [TestClass]
    public class ReservationSqlDalTests
    {
        private TransactionScope tran;
        private string connectionString = @"Data Source =.\sqlexpress;Initial Catalog = NationalParkReservation; Integrated Security = True";
        private int reservationCount = 0;
        private int reservationInCampgroundCount = 0;
        private int reservationId;

        //INITIALIZE
        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd;

                // Get the number of Reservations
                cmd = new SqlCommand("SELECT count(*) FROM reservation;", connection);
                reservationCount = (int)cmd.ExecuteScalar();

                //Get the number of Reservations in the campground
                cmd = new SqlCommand("SELECT count(*) FROM reservation JOIN site ON site.site_id = reservation.site_id " + 
                    "JOIN campground ON campground.campground_id = site.campground_id WHERE campground.campground_id = 1;", connection);
                reservationInCampgroundCount = (int)cmd.ExecuteScalar();

                //Get reservation Id
                cmd = new SqlCommand("INSERT INTO reservation (site_id, name, from_date, to_date) " +
                    "VALUES (1, 'Test Reservation', '2000-01-01', '2000-01-04'); SELECT CAST(SCOPE_IDENTITY() as int);", connection);
                reservationId = (int)cmd.ExecuteScalar();
                reservationCount++;
                reservationInCampgroundCount++;
            }
        }

        /*
        * CLEANUP
        * Rollback the existing transaction.
        */
        [TestCleanup]
        public void Cleanup()
        {
            tran.Dispose();
        }

        [TestMethod]
        public void GetAllReservationsTest()
        {
            //Arange
            ReservationSqlDal reservationSqlDal = new ReservationSqlDal(connectionString);

            //ACT
            List<Reservation> reservations = reservationSqlDal.GetAllReservations();

            //Assert
            Assert.IsNotNull(reservations, "Reservations list is empty!");
            Assert.AreEqual(reservationCount, reservations.Count, $"Expected a count of {reservationCount} for reservations");

            bool found = false;
            foreach (Reservation reservation in reservations)
            {
                if (reservation.Name == "Test Reservation")
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "Could not find Test Reservation named Test Reservation");
        }

        [TestMethod]
        public void GetAllReservationsFromCampgroundTest()
        {
            //Arrange
            ReservationSqlDal reservationSqlDal = new ReservationSqlDal(connectionString);

            //Act
            List<Reservation> reservations = reservationSqlDal.GetAllReservationsFromCampground(1);

            //Assert
            Assert.IsNotNull(reservations, "Reservations list is empty!");
            Assert.AreEqual(reservationInCampgroundCount, reservations.Count, $"Expected a count of {reservationInCampgroundCount} for reservations");

            bool found = false;
            foreach (Reservation reservation in reservations)
            {
                if (reservation.Name == "Test Reservation")
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "Could not find Test Reservation named Test Reservation");

        }

        [TestMethod]
        public void MakeReservationTest()
        {
            //Arrange
            ReservationSqlDal reservationSqlDal = new ReservationSqlDal(connectionString);
            int siteId = 1;
            string name = "Test Reservation";
            DateTime checkInDate = new DateTime(2100, 1, 1);
            DateTime checkOutDate = new DateTime(2100, 1, 5);

            //Act
            int reservationId = reservationSqlDal.MakeReservation(siteId, name, checkInDate, checkOutDate);

            //Assert
            Assert.AreNotEqual(0, reservationId);
        }
    }
}
