using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Park
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime EstablishDate { get; set; }
        public int Area { get; set; }
        public int Visitors { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Name + "\n" +
                "Location: ".PadRight(20) + Location + "\n" +
                "Established: ".PadRight(20) + EstablishDate.ToShortDateString() + "\n" +
                "Area: ".PadRight(20) + Area + " Acres\n" +
                "Annual Visitors: ".PadRight(20) + Visitors + "\n\n" +
                Description + "\n";
            
        }
    }

    
}
