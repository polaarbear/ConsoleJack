using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleJack
{
    //This class holds a list that can have cards added and removed (such as a hand or the deck)
    class CardList
    {
        //The list of cards
        public List<Card> Cards { get; private set; }

        //Initializes the empty list of cards
        public CardList()
        {
            Cards = new List<Card>(); //Initializes the list of card
        }

        //Adds a card to the hand or back into the deck
        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        //Removes a card from the list either to remove it from a hand or to deal it
        public Card RemoveCard(int cardIndex)
        {
            Card removedCard = Cards[cardIndex];
            Cards.RemoveAt(cardIndex);
            return removedCard;
        }
    }
}
