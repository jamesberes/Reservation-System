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
    public class ParkSqlDalTests
    {
        private TransactionScope tran;
        private string connectionString = @"Data Source =.\sqlexpress;Initial Catalog = NationalParkReservation; Integrated Security = True";
        private int parkCount = 0;
        private int parkId;

        //INITIALIZE
        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd;

                // Get the number of Parks
                cmd = new SqlCommand("SELECT count(*) FROM park;", connection);
                parkCount = (int)cmd.ExecuteScalar();

                //Get park Id
                cmd = new SqlCommand("INSERT INTO park (name, location, establish_date, area, visitors, description) " +
                    "VALUES ('Test Park', 'Columbus', '1981-01-01', 50, 929292, 'long description'); SELECT CAST(SCOPE_IDENTITY() as int);", connection);
                parkId = (int)cmd.ExecuteScalar();
                parkCount++;
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
        public void GetAllParksTest()
        {
            //Arange
            ParkSqlDal parkSqlDal = new ParkSqlDal(connectionString);

            //ACT
            List<Park> parks = parkSqlDal.GetAllParks();

            //Assert
            Assert.IsNotNull(parks, "Parks list is empty!");
            Assert.AreEqual(parkCount, parks.Count, $"Expected a count of {parkCount} for parks");

            bool found = false;
            foreach (Park park in parks)
            {
                if (park.Name == "Test Park")
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "Could not find Test Park named Test Park");
        }
    }
}