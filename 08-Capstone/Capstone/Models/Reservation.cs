using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime CreateDate { get; set; }

        public override string ToString()
        {
            return Name.ToString() + "\n" +
                "Id".PadRight(15) + Id + "\n" +
                "Site Id".PadRight(15) + SiteId + "\n" +
                "Check-in".PadRight(15) + FromDate.ToShortDateString() + "\n" +
                "Check-out".PadRight(15) + ToDate.ToShortDateString() + "\n" +
                "Creation Date".PadRight(15) + CreateDate + "\n";
        }
    }
}
