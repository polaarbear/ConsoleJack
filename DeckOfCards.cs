using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleJack
{
    /* This class combines 52 cards into a single complete deck from which cards
     * can be dealt and then returned */
    class DeckOfCards : CardList
    {
        #region Constructor/Deck Creation

        //Constructs the deck of cards
        public DeckOfCards()
        {
            PrintDeck();
        }

        //Calls PrintSuit() 4 times to prepare the deck
        private void PrintDeck()
        {
            for (int suit = 1; suit <= 4; suit++) //Iterates for all 4 suits
            {
                switch(suit)
                {
                    case 1:
                        PrintSuit('C');  //Creates 13 clubs
                        break;
                    case 2:
                        PrintSuit('D');  //Creates 13 diamonds
                        break;
                    case 3:
                        PrintSuit('S');  //Creates 13 spades
                        break;
                    case 4:
                        PrintSuit('H');  //Creates 13 hearts
                        break;
                }
            }
        }

        //Prints 13 cards with correct face and suit values
        private void PrintSuit(char suit)
        {
            for (int faceValue = 1; faceValue <= 13; faceValue++)  //Iterates 13 times (Ace-King)
            {
                Cards.Add(new Card(faceValue, suit));
            }
        }

        #endregion

        #region Deck Management

        //Deals the top card from the deck by calling the base RemoveCard method with a zero index
        public Card DealCard()
        {
            return RemoveCard(0);
        }

        //A standard implementation of the Fisher-Yates shuffle algorithm
        public void ShuffleDeck()
        {
            for (int currentCard = 0; currentCard < Cards.Count; currentCard++)
            {
                int selectedCard = CasinoDoor.RANDOM.Next(currentCard, Cards.Count - 1);
                if (currentCard == selectedCard)
                    continue;
                else
                {
                    Card shuffleCard = Cards[selectedCard];
                    Cards[selectedCard] = Cards[currentCard];
                    Cards[currentCard] = shuffleCard;
                }
            }
        }

        #endregion
    }
}
