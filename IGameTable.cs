using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleJack
{
    /* The GameTable class is a template for each game table*/
    interface IGameTable
    {
        Player HumanPlayer { get; set; } //Every game table must have a human player

        //DisplayTable is called to redraw the console as needed for each game. allHandsRevealed can hide or show opponent hands
        void DisplayTable(bool allHandsRevealed);

        //Each game will use this as a starting point for other functions
        void PlayGame();

        //Each table will have a method to prompt users for various input errors
        void DisplayError(string errorType);
    }
}
