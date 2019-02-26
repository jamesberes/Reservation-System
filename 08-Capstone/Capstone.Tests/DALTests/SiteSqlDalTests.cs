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
    public class SiteSqlDalTests
    {
        private TransactionScope tran;
        private string connectionString = @"Data Source =.\sqlexpress;Initial Catalog = NationalParkReservation; Integrated Security = True";
        private int siteCount = 0;
        private int availableSiteCount = 0;
        private int siteId;
        private int reservationId;

        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd;

                // Get the number of Sites
                cmd = new SqlCommand("SELECT count(*) FROM site WHERE site.campground_id = 1;", connection);
                siteCount = (int)cmd.ExecuteScalar();

                //Get site Id
                cmd = new SqlCommand("INSERT INTO site (campground_id, site_number, max_occupancy, accessible, max_rv_length, utilities) " +
                    "VALUES (1, 1, 10, 0, 0, 0); SELECT CAST(SCOPE_IDENTITY() as int);", connection);
                siteId = (int)cmd.ExecuteScalar();
                siteCount++;

                //Insert some reservations
                cmd = new SqlCommand("INSERT INTO reservation (site_id, name, from_date, to_date) " +
                    "VALUES (1, 'Test reservation', '2100-01-01', '2100-05-31'); SELECT CAST(SCOPE_IDENTITY() as int);", connection);
                reservationId = (int)cmd.ExecuteScalar();

                //Check available sites
                cmd = new SqlCommand("Select COUNT(*) FROM (" + 
                    "Select site.site_number " +
                    "From reservation " +
                    "JOIN site ON reservation.site_id = site.site_id " +
                    "JOIN campground ON site.campground_id = campground.campground_id " +
                    "where campground.campground_id = 1 " +
                    "AND '2100-01-01' NOT BETWEEN reservation.from_date AND reservation.to_date " +
                    "AND '2100-05-31' NOT BETWEEN reservation.from_date AND reservation.to_date " +
                    "GROUP BY site.site_number" + 
                    ")As result;", connection);
                availableSiteCount = (int)cmd.ExecuteScalar();
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
        public void GetAllSitesFromCampgroundTest()
        {
            //Arange
            SiteSqlDal siteSqlDal = new SiteSqlDal(connectionString);

            //ACT
            List<Site> sites = siteSqlDal.GetAllSitesFromCampground(1);

            //Assert
            Assert.IsNotNull(sites, "Sites list is empty!");
            Assert.AreEqual(siteCount, sites.Count, $"Expected a count of {siteCount} for sites");

            bool found = false;
            foreach (Site site in sites)
            {
                if (site.SiteNumber == 1)
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "Could not find Test Site named Test Site");
        }
   

        [TestMethod]
        public void GetAvailableSitesFromCampgroundTest()
        {
            //Arrange
            SiteSqlDal siteSqlDal = new SiteSqlDal(connectionString);

            //Act
            DateTime checkIn = new DateTime(2100, 1, 1);
            DateTime checkOut = new DateTime(2100, 5, 1);
            List<Site> sites = siteSqlDal.GetAvailableSitesFromCampground(1, checkIn, checkOut);

            //Assert
            Assert.IsNotNull(sites, "Sites list is empty!");

            //Since GetAvailableSitesFromCampground caps at 5, cap our site-count at 5 here
            if (availableSiteCount > 5)
            {
                availableSiteCount = 5;
            }

            Assert.AreEqual(availableSiteCount, sites.Count, $"Expected a count of {availableSiteCount} for sites");
        }

        //[TestMethod]
        //public void GetAvailableSitesFromParkTest()
        //{
        //    //Todo Bonus method
        //}
    }
}
