using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


namespace Capstone.Models
{
    public class Campground
    {
        public int Id { get; set; }
        public int ParkId { get; set; }
        public string Name { get; set; }
        public int MonthOpenFrom { get; set; }
        public int MonthOpenTo { get; set; }
        public decimal DailyFee { get; set; }

        public override string ToString()
        {
            return "#" + Id.ToString().PadRight(4) +
                Name.ToString().PadRight(35) +
                CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(MonthOpenFrom).PadRight(20) +
                CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(MonthOpenTo).PadRight(20) +
                DailyFee.ToString("C");
        }
    }
}
