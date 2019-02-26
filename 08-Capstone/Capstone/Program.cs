using Capstone;
using System;

namespace capstone
{
    class Program
    {
        static void Main(string[] args)
        {
            NationalParkReservationCLI cli = new NationalParkReservationCLI();
            cli.RunCLI();
        }
    }
}
