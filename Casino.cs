using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleJack
{
    /* The Casino class allows the user to select a game to play and will load
     * the appropriate rule set for the chosen game. */
    class Casino
    {
        //This method is called every time the screen is refreshed

        #region Constructor and Table Selection

        //The casino constructor calls the game selection method to initialize play
        public Casino()
        {
            Console.WriteLine("ConsoleJack is starting. Good luck!");
            System.Threading.Thread.Sleep(1750);
            BlackJackTable blackJackTable = new BlackJackTable(new Player(1000));
        }

        #endregion
    }
}
