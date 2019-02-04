using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleJack
{
    /* This interface is effectively the dealer for each table
     * Each game will have different rules, this seemed the most
     effective way to specialize the dealing and gathering functions*/
    interface IDealer
    {
        DeckOfCards Deck { get; set; } //Each Dealer will need a deck of cards

        void DealHands(); //Deals the cards based on the rules of the current game

        void GatherDeck(); //This allows the cards to be gathered back into the deck
    }        
}
