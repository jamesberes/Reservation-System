using System;
using System.Collections.Generic;
using System.Text;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone
{
    public class NationalParkReservationCLI
    {
        private const string DatabaseConnection = @"Data Source=.\sqlexpress;Initial Catalog=NationalParkReservation;Integrated Security=True";

        public void RunCLI()
        {
            Welcome();
            AllParksScreen();
        }

        public void AllParksScreen()
        {
            bool done = false;
            while (!done)
            {
                Console.Clear();
                Console.WriteLine("Select a Park for Further Details: ");

                ParkSqlDal parkSqlDal = new ParkSqlDal(DatabaseConnection);
                List<Park> parks = parkSqlDal.GetAllParks();
                foreach (Park park in parks)
                {
                    Console.WriteLine($"{park.Id}) {park.Name} ");
                }
                Console.WriteLine("Q) Quit\n");

                string userinput = Console.ReadLine().ToUpper();
                bool userEnteredNumber = int.TryParse(userinput, out int parkIdEntry);
                Park selectedPark = GetParkFromListById(parkIdEntry, parks);

                if (userEnteredNumber)
                {
                    if (selectedPark != null)
                    {
                        ParkInformationScreen(selectedPark);
                    }
                    else
                    {
                        Console.WriteLine("Park Id not found! Press any key to try again");
                        Console.ReadKey();
                    }
                }
                else
                {
                    if (userinput == "Q")
                    {
                        done = true;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid entry, press any key to try again");
                        Console.ReadKey();
                    }
                }
            }
        }

        public void ParkInformationScreen(Park selectedPark)
        {
            bool done = false;
            while (!done)
            {
                Console.Clear();
                Console.WriteLine("Park Information Screen: ");
                Console.WriteLine(selectedPark.ToString());
                Console.WriteLine("Select a Command" + "\n\t1) View Campgrounds" + "\n\t2) Search for Reservation" + "\n\t3) Return to Previous Screen\n");
                string userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1":
                        CampgroundsInformationScreen(selectedPark);
                        break;

                    case "2":
                        SearchForReservationByIdScreen();
                        break;

                    case "3":
                        done = true;
                        break;

                    default:
                        Console.WriteLine("Please enter a valid input, press any key to try again");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public void CampgroundsInformationScreen(Park park)
        {
            bool done = false;
            while (!done)
            {
                Console.Clear();
                Console.WriteLine("Park Campgrounds: ");
                PrintAllCampgroundInfoInPark(park);
                Console.WriteLine("\nSelect a Command" + "\n\t1) Search for Available Reservation" + "\n\t2) Return to Previous Screen\n");
                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        SearchForAvailableReservationScreen(park);
                        break;
                    case "2":
                        done = true;
                        break;
                    default:
                        Console.WriteLine("Please enter a valid input, press any key to try again");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public void SearchForReservationByIdScreen()
        {
            ReservationSqlDal reservationSqlDal = new ReservationSqlDal(DatabaseConnection);
            List<Reservation> reservations = reservationSqlDal.GetAllReservations();

            Console.WriteLine("Reservation Search:\n");
            int reservationId = CLIHelper.GetInteger("Please enter your reservation ID #:");
            Reservation selectedReservation = GetReservationById(reservationId, reservations);

            if (selectedReservation != null)
            {
                Console.WriteLine($"Reservation {reservationId} found...");
                Console.WriteLine(selectedReservation.ToString());
            }
            else
            {
                Console.WriteLine("Reservation not found!");
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        public void SearchForAvailableReservationScreen(Park park)
        {
            SiteSqlDal siteSqlDal = new SiteSqlDal(DatabaseConnection);
            CampgroundSqlDal campgroundSqlDal = new CampgroundSqlDal(DatabaseConnection);
            ReservationSqlDal reservationSqlDal = new ReservationSqlDal(DatabaseConnection);
            List<Campground> campgrounds = campgroundSqlDal.GetAllCampgroundsFromPark(park.Id);

            //Search for valid campground
            bool done = false;
            while (!done)
            {
                Console.Clear();
                Console.WriteLine("Search for Campground Reservation: ");
                PrintAllCampgroundInfoInPark(park);

                int userInputCampgroundId = CLIHelper.GetInteger("\nWhich Campground number (Enter 0 to cancel)?");
                if (userInputCampgroundId == 0)
                {
                    Console.WriteLine("Cancelled! Press any key to continue.");
                    Console.ReadKey();
                    return;
                }

                if (GetCampgroundById(userInputCampgroundId, campgrounds) == null)
                {
                    Console.WriteLine("Not a valid campground! Press any key to continue.");
                    Console.ReadKey();
                    return;
                }

                //Once valid campground has been chosen --> Get good dates for query
                DateTime checkIn = CLIHelper.GetDateTime("Check-in date: ");
                DateTime checkOut = CLIHelper.GetDateTime("Check-out date: ");
                List<Site> availableSitesFromCampgrounds = new List<Site>(); 
                bool gotDates = false;
                bool showReservationPrompt = false;
                while (!gotDates)
                {
                    availableSitesFromCampgrounds = siteSqlDal.GetAvailableSitesFromCampground(userInputCampgroundId, checkIn, checkOut);
                    if (checkOut.CompareTo(checkIn) <= 0)
                    {
                        Console.WriteLine("Cannot check-out earlier or same day as check-in.  Press any key to continue");
                        Console.ReadKey();
                        showReservationPrompt = false;
                        gotDates = true;
                        //could allow user a choice to return or enter new dates
                    }
                    else if (availableSitesFromCampgrounds.Count < 1)
                    {
                        string dateReset = CLIHelper.GetString("\nThere are no available sites. \nWould you like to enter an alternate date range?\n\tYes or No?").ToLower();
                        if (dateReset == "yes" || dateReset == "y")
                        {
                            Console.WriteLine();
                            checkIn = CLIHelper.GetDateTime("Check-in date: ");
                            checkOut = CLIHelper.GetDateTime("Check-out date: ");
                            gotDates = false;
                            
                        }
                        else if (dateReset == "no" || dateReset == "n")
                        {
                            gotDates = true;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Try again");
                            gotDates = false;
                        }
                    }
                    else
                    {
                        showReservationPrompt = true;
                        gotDates = true;
                    }
                }
                if (showReservationPrompt)
                {
                    int daysReserved = checkOut.Subtract(checkIn).Days;
                    Console.WriteLine("Site Id".PadRight(10) + "Max Occup.".PadRight(15) + "Accessible?".PadRight(15) + "Max RV Length".PadRight(20) + "Utility".PadRight(15) + "Cost\n");
                    foreach (Site site in availableSitesFromCampgrounds)
                    {
                        Console.WriteLine(site.GetPrintString(daysReserved, GetCampgroundById(userInputCampgroundId, campgrounds).DailyFee));
                    }
                    Console.WriteLine();
                
                    MakeReservationPrompts(checkIn, checkOut, availableSitesFromCampgrounds);
                    done = true;
                }
            }
        }

        public void MakeReservationPrompts(DateTime checkInDate, DateTime checkOutDate, List<Site> availableSites)
        {
            ReservationSqlDal reservationSqlDal = new ReservationSqlDal(DatabaseConnection);
            int siteToReserve = CLIHelper.GetInteger("Which site should be reserved (enter 0 to cancel)?");

            //quit if zero
            if (siteToReserve == 0)
            {
                Console.WriteLine("Cancelled! Press any key to continue.");
                return;
            }

            //check if non-zero response is actually available
            bool isValidSite = false;
            foreach (Site site in availableSites)
            {
                if (site.Id == siteToReserve)
                {
                    isValidSite = true;
                }
            }

            //display message if site chosen was invalid
            if (!isValidSite)
            {
                Console.WriteLine("\nInvalid site or site not available, please try another campsite. \nPress any key to continue: ");
            }
            else
            {
                string reservationName = CLIHelper.GetString("What name should the reservation be made under?");
                int confirmationId = reservationSqlDal.MakeReservation(siteToReserve, reservationName, checkInDate, checkOutDate);
                if (confirmationId != 0)
                {
                    Console.WriteLine($"The reservation has been made and the confirmation id is {confirmationId}");
                }
                else
                {
                    Console.WriteLine($"Error: The reservation was not made");
                }
                Console.WriteLine("\nPress any key to continue");
            }
            Console.ReadKey();
            return;
        }

        public void PrintAllCampgroundInfoInPark(Park park)
        {
            Console.WriteLine(park.Name + "\n");
            Console.WriteLine("Id".PadRight(5) + "Name".PadRight(35) + "Open".PadRight(20) + "Close".PadRight(20) + "Daily Fee\n");

            CampgroundSqlDal campgroundSqlDal = new CampgroundSqlDal(DatabaseConnection);
            List<Campground> campgrounds = campgroundSqlDal.GetAllCampgroundsFromPark(park.Id);
            foreach (Campground campground in campgrounds)
            {
                Console.WriteLine(campground);
            }
        }

        public Park GetParkFromListById(int parkId, List<Park> parks)
        {
            Park result = null;
            foreach (Park park in parks)
            {
                if (park.Id == parkId)
                {
                    result = park;
                }
            }
            return result;
        }

        public Campground GetCampgroundById(int campgroundId, List<Campground> campgrounds)
        {
            Campground result = null;
            foreach (Campground campground in campgrounds)
            {
                if (campground.Id == campgroundId)
                {
                    result = campground;
                }
            }
            return result;
        }

        public Site GetCampsiteById(int campsiteId, List<Site> sites)
        {
            Site result = null;
            foreach (Site campsite in sites)
            {
                if (campsite.Id == campsiteId)
                {
                    result = campsite;
                }
            }
            return result;
        }

        public Reservation GetReservationById(int reservationId, List<Reservation> reservations)
        {
            Reservation result = null;
            foreach (Reservation reservation in reservations)
            {
                if (reservation.Id == reservationId)
                {
                    result = reservation;
                }
            }
            return result;
        }

        //TODO Bonus : park wide availability.         
        public void ParkWideReservations(Park park)
        {
            //calls GetAvailableSitesFromPark from SiteSqlDal
        }

        public void Welcome()
        {
            Console.WriteLine("\n\n\n\n\n\t\t\t\t\t               ,@@@@@@@,\n" +
            "\t\t\t\t\t       ,,,.   ,@@@@@@/@@,  .oo8888o.\n" +
            "\t\t\t\t\t    ,&%%&%&&%,@@@@@/@@@@@@,8888\\88/8o\n" +
            "\t\t\t\t\t   ,%&\\%&&%&&%,@@@\\@@@/@@@88\\88888/88'\n" +
            "\t\t\t\t\t   %&&%&%&/%&&%@@\\@@/ /@@@88888\\88888'\n" +
            "\t\t\t\t\t   %&&%/ %&%%&&@@\\ V /@@' `88\\8 `/88'\n" +
            "\t\t\t\t\t   `&%\\ ` /%&'    |.|        \\ '|8'\n" +
            "\t\t\t\t\t       |o|        | |         | |\n" +
            "\t\t\t\t\t       |.|        | |         | |\n" +
            "\t\t\t\t\t     \\/ ._\\//_/__/  ,\\_//__\\\\/.  \\_//__/_");

            Console.WriteLine("\n\t\t\t\t\tWelcome to the National Parks Reservation System!");
            Console.Write("\n\t\t\t\t\t\tPress any key to continue...");
            Console.ReadKey();

        }
    }
}