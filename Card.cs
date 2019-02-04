using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleJack
{
    /* This class represents a single playing card which can be combined
     * in a list to create a complete deck of playing cards */
    class Card
    {
        public int Value { get; private set; } //This is the face value of the card 1-13 = Ace-King
        public char Suit { get; private set; } //Only four suit values are valid, 'C'lubs, 'D'iamonds, 'H'earts, 'S'pades

        //Initializes the card values
        public Card(int value, char suit)
        {
            Value = value;
            Suit = suit;
        }

        /* Prints the information for this card directly to the console.
         * Uses UTF-8 to produce suit values.  Some fonts can't render
         * the suit values correctly. */
        public void PrintCardInfo()
        {
            string cardInfo = "";
            switch(Value)
            {
                case 1:
                    cardInfo += "A"; //Aces use an A
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    cardInfo += Value.ToString(); //2-9 use an integer
                    break;
                case 10:
                    cardInfo += "X"; //10's use an X to maintain single-character formatting
                    break;
                case 11:
                    cardInfo += "J"; //Jacks use a J
                    break;
                case 12:
                    cardInfo += "Q"; //Queens use a Q
                    break;
                case 13:
                    cardInfo += "K"; //Kings use a K
                    break;
            }
            switch(Suit)
            {
                case 'C':
                    cardInfo += "\u2663"; //The utf value for clubs
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case 'D':
                    cardInfo += "\u2666"; //The utf value for diamonds
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case 'S':
                    cardInfo += "\u2660"; //The utf value for spades
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case 'H':
                    cardInfo += "\u2665"; //The utf value for hearts
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }
            Console.Write(cardInfo + " ");
            CasinoDoor.RestoreDefaultColors();
        }
    }
}
