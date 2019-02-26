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
    public class CampgroundSqlDalTests
    {
        private TransactionScope tran;
        private string connectionString = @"Data Source =.\sqlexpress;Initial Catalog = NationalParkReservation; Integrated Security = True";
        private int campgroundCount = 0;
        private int campgroundId;
        
        //INITIALIZE
        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd;
                
                // Get the number of Campgrounds
                cmd = new SqlCommand("SELECT count(*) FROM campground Join park on park.park_id = campground.park_id WHERE park.park_id = 1", connection);
                campgroundCount = (int)cmd.ExecuteScalar();

                //Get campground Id
                cmd = new SqlCommand("INSERT INTO campground (park_id, name, open_from_mm, open_to_mm, daily_fee) " +
                    "VALUES (1, 'Test Campground', 1, 12, 20.00); SELECT CAST(SCOPE_IDENTITY() as int);", connection);
                campgroundId = (int)cmd.ExecuteScalar();
                campgroundCount++;
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
        public void GetAllCampgroundsFromParkTest() //List<Campground> GetAllCampgroundsFromPark(int parkId)
        {
            //Arange
            CampgroundSqlDal campgroundSqlDal = new CampgroundSqlDal(connectionString);
            
            //ACT
            List<Campground> campgrounds = campgroundSqlDal.GetAllCampgroundsFromPark(1);

            //Assert
            Assert.IsNotNull(campgrounds, "Campgrounds list is empty!");
            Assert.AreEqual(campgroundCount, campgrounds.Count, $"Expected a count of {campgroundCount} for campgrounds");

            //Insert Assert
            bool found = false;
            foreach (Campground campground in campgrounds)
            {
                if (campground.Name == "Test Campground")
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "Could not find Test Campground named Test Campground");
        }
    }
}