using System;

namespace ConsoleJack
{
    class CasinoDoor
    {
        private static Casino CORESINO;  //This is the lone instance of the Casino class

        private static int SLEEPTIMER = 1250; //This can be adjusted as needed to read game text

        public static Random RANDOM = new Random(); //All games share a global random number generator

        //The main entry point to the casino
        static void Main(string[] args)
        {
            SetCasinoParameters();
            EnterCasino();
        }

        //Sets initial parameters to display the console appropriately
        static void SetCasinoParameters()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; //UTF8 allows for card suit values to be placed into the text
            Console.SetWindowSize(64, 32); //This size is appropriate for the initial games. May be updated later

            Console.BackgroundColor = ConsoleColor.DarkGreen; //The backdrop is dark-green to match the felt of a card table
            Console.ForegroundColor = ConsoleColor.White; //White text is easy to read on the background. Black will be reserved for card values
            Console.Clear(); //Clears the console to apply the new background color.
        }

        //Instantiates the casino
        static void EnterCasino()
        {
            CORESINO = new Casino();
        }

        //Pauses the current thread to allow the player to absorb information
        public static void WaitForDisplay()
        {
            System.Threading.Thread.Sleep(SLEEPTIMER);
        }
        //Restores the standard terminal colors for the application after writing specially-colored text
        public static void RestoreDefaultColors()
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
