using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleJack
{
    /* The player class manages the user's bank. Hands are managed by the CardList class as
     * games such as BlackJack require multiple hands and other special considerations */
    class Player
    {
        public float Bank { get; private set; } //This is the player's bankroll aka available funds

        //The initial bank size is specified when the player is initialized
        public Player(float startingBank)
        {
            Bank = startingBank;
        }

        /* Placing a bet removes the inputted amount from the bank
         * and returns it to be added to a pot if necessary */
        public float PlaceBet(float amount)
        {
            Bank -= amount;
            return amount;
        }

        //Adds winnings into the player's bank
        public void AddToBank(float earnings)
        {
            Bank += earnings;
        }
    }
}
