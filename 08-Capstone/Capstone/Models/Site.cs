using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Site
    {
        public int Id { get; set; }
        public int CampgroundId { get; set; }
        public int SiteNumber { get; set; }
        public int MaxOccupancy { get; set; }
        public bool Accessible { get; set; }
        public int MaxRv { get; set; }
        public bool Utilities { get; set; }

        public string GetPrintString(int numberOfDaysReserved, decimal dailyFee)
        {
            decimal reservationCost = numberOfDaysReserved * dailyFee;

            string yesNoForAccessible = "";
            if (Accessible)
            {
                yesNoForAccessible = "Yes";
            }
            else
            {
                yesNoForAccessible = "No";
            }

            string maxRVNA = "";
            if (MaxRv == 0)
            {
                maxRVNA = "N/A";
            }
            else
            {
                maxRVNA = MaxRv.ToString();
            }

            string utilityYesNA = "";
            if (Utilities)
            {
                utilityYesNA = "Yes";
            }
            else
            {
                utilityYesNA = "N/A";
            }
            //edit changed siteNumber to Id
            return Id.ToString().PadRight(10) + MaxOccupancy.ToString().PadRight(15) + 
                yesNoForAccessible.PadRight(15) + maxRVNA.PadRight(20) + utilityYesNA.PadRight(15) + reservationCost.ToString("C");
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Site otherObject = (Site)obj;

            return (otherObject.Id == this.Id &&
                otherObject.CampgroundId == this.CampgroundId &&
                otherObject.SiteNumber == this.SiteNumber &&
                otherObject.MaxOccupancy == this.MaxOccupancy &&
                otherObject.Accessible == this.Accessible &&
                otherObject.MaxRv == this.MaxRv &&
                otherObject.Utilities == this.Utilities);
        }
    }
}
