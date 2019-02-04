using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleJack
{
    abstract class BlackJackRules : IDealer
    {
        public DeckOfCards Deck { get; set; } //The deck is managed by the IDealer functions

        public Player HumanPlayer { get; set; } //The human player

        protected CardList dealerHand; //A list of cards to represent the dealer's hand
        protected List<CardList> playerHands; //A place to store multiple hands in the event of a split
        protected List<bool> frozenHands; //A list to track which hands have been frozen by doubling down, staying, or busting
        protected List<float> playerBets; //A list of the bets tied to each individual hand if the player ends up splitting them

        //The BlackJackRules are initialized alongside the BlackJackTable
        public BlackJackRules()
        {
            dealerHand = new CardList(); //Initializes the empty dealer hand
            playerHands = new List<CardList>(); //Initializes an empty list of player hands
            frozenHands = new List<bool>();
            playerBets = new List<float>();
        }

        #region Wager/Deal

        /* This is the pre-bet wager.  Players choose the amount they wish to wager
         * at the beginning of each hand with opportunities to increase it via other
         * means later */
        protected bool InitialWagerPrompt()
        {
            float wagerAmount; //This is used to store the player input from the console

            Console.Write("How much would you like to wager?\n(Min = $1.00, Max = " + HumanPlayer.Bank.ToString("c2") + ")\n"); //Prompts the player to wager an appropriate amount
            if (float.TryParse(Console.ReadLine(), out wagerAmount))
            {
                if (wagerAmount > 0 && wagerAmount <= HumanPlayer.Bank) //The wager has to be between 1 and 100 dollars
                {
                    Console.Write("\nYour wager was ");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(wagerAmount.ToString("c2")); //The wager is confirmed
                    CasinoDoor.RestoreDefaultColors();
                    HumanPlayer.PlaceBet(wagerAmount); //The wager amount is removed from the player's bank
                    playerBets.Add(wagerAmount); //Adds the initial wager to the list of wagers
                    return true;
                }
            }
            return false;
        }

        /* Shuffles the deck and deals the initial 4 cards, 2 to the dealer and 2 to the
         * human player.*/
        public void DealHands()
        {
            Console.WriteLine("Shuffling deck...");
            Deck.ShuffleDeck();
            CasinoDoor.WaitForDisplay();
            Console.WriteLine("Dealing hands...");
            for (int startingCards = 0; startingCards < 2; startingCards++)
            {
                if (playerHands.Count == 0)
                {
                    playerHands.Add(new CardList());
                    frozenHands.Add(false);
                }
                playerHands[0].AddCard(Deck.DealCard());
                dealerHand.AddCard(Deck.DealCard());
            }
        }

        #endregion

        #region Hand Checks

        /* This method adds the values in the hand and returns it as an int.
         * If the hand goes over 21 it checks to see if there is an ace present
         * to adjust the score before returning it. */
        protected int CheckHandScore(CardList hand)
        {
            int handScore = 0;
            int acesPresent = 0; //Counts the aces present to adjust for busted hands
            foreach (Card card in hand.Cards)
            {
                if (card.Value == 1)
                {
                    acesPresent++;
                    handScore += 11;
                }
                else if (card.Value > 1 && card.Value < 10)
                    handScore += card.Value;
                else
                    handScore += 10;
            }
            //Adjusts the score as necessary when there are one or more aces present
            while (handScore > 21 && acesPresent > 0)
            {
                handScore -= 10;
                acesPresent--;
            }

            return handScore;
        }

        /* Returns true if all hands have been frozen, either by busting, doubling
         * down, or choosing to stay the hand*/
        protected bool AllHandsFrozen()
        {
            foreach (bool isHandStayed in frozenHands)
            {
                if (isHandStayed == false)
                    return false;
            }
            return true;
        }

        /* Returns true if all hands have a value greater than 21, otherwise
         * returns false*/
        protected bool AllHandsBusted()
        {
            foreach (CardList hand in playerHands)
            {
                if (CheckHandScore(hand) <= 21)
                    return false;
            }
            Console.WriteLine("All hands went BUST!\nAll wagers lost.");
            return true;
        }

        //Returns true if the hand is a blackjack otherwise returns false
        protected bool CheckBlackJack(CardList hand, int handIndex)
        {
            if (hand.Cards.Count == 2 && CheckHandScore(hand) == 21)
            {
                frozenHands[handIndex] = true;
                return true;
            }
            return false;
        }

        #endregion

        #region Splitting Hands

        /* This returns true if the hand is exactly 2 cards with paired
         * face values */
        protected bool CheckSplit(CardList hand)
        {
            if (hand.Cards[0].Value == hand.Cards[1].Value)
                return true;
            else
                return false;
        }
        /* This prompt is displayed to the user if their hand is valid to be
         * split into two*/
        protected bool SplitHandPrompt(CardList hand, int handIndex)
        {
            Console.Write("Split hand " + (handIndex + 1).ToString() + "?\n(Y/N): ");
            char userInput = Console.ReadKey(true).KeyChar.ToString().ToUpper()[0]; //Stores the next key pressed as an uppercase char
            if (userInput == 'Y')
            {
                SplitHand(hand, handIndex); //Splits the paired cards into two hands
                return true;
            }
            else if (userInput == 'N')
            {
                Console.WriteLine("Hand not split.\n");
                DoubleDownPrompt(hand, handIndex); //To avoid a never-ending loop of hand split offers
                return true;
            }
            else
            {
                return false;
            }

        }
        //This performs the actual action of splitting the hand if the player so chooses.
        private void SplitHand(CardList hand, int handIndex)
        {
            if (playerBets[handIndex] <= HumanPlayer.Bank)
            {
                Console.WriteLine("Split bet placed.\n");
                playerBets.Add(playerBets[handIndex]);
            }
            else
            {
                Console.WriteLine("Not enough funds, bet " + HumanPlayer.Bank.ToString() + ".00 on split hand\n");
                playerBets.Add(HumanPlayer.Bank);
            }
            playerHands.Add(new CardList());
            frozenHands.Add(false);
            int newHandIndex = playerHands.Count - 1; //Finds the newly created hand
            playerHands[newHandIndex].AddCard(playerHands[handIndex].Cards[1]); //Copies one of the original cards
            playerHands[handIndex].RemoveCard(1); //Removes the copied card

            //Fill both hands
            playerHands[handIndex].AddCard(Deck.DealCard());
            playerHands[newHandIndex].AddCard(Deck.DealCard());
        }

        #endregion

        #region Doubling Down/Hit

        /* This method initializes the double down process.  Any hand of exactly
         * 2 cards can Double Down. Returning true signifies proper user input.
         * Incorrect input will display an error before forcing the game to loop
         * back to here. The split prompt calls this immediately if the player
         * chooses not to split to avoid a never-ending loop of split prompts*/
        protected bool DoubleDownPrompt(CardList hand, int handIndex)
        {
            Console.Write("Double down on hand " + (handIndex + 1).ToString() + "?\n(Y/N): ");
            char userInput = Console.ReadKey(true).KeyChar.ToString().ToUpper()[0]; //Stores the next key pressed as an uppercase char
            if (userInput == 'Y')
            {
                DoubleDown(hand, handIndex); //Adds a card to the hand
                return true;
            }
            else if (userInput == 'N')
            {
                Console.WriteLine("Passed on double down\n");
                HitOrStay(hand, handIndex); //To avoid a never-ending loop of double down offers
                return true;
            }
            return false;
        }
        //This is called by DoubleDownPrompt() when the user chooses to double down
        private void DoubleDown(CardList hand, int handIndex)
        {
            if (playerBets[handIndex] <= HumanPlayer.Bank)
            {
                Console.WriteLine("Double down bet placed.\n");
                playerBets[handIndex] += HumanPlayer.PlaceBet(playerBets[handIndex]);
            }
            else
            {
                Console.WriteLine("Not enough funds, bet " + HumanPlayer.Bank.ToString() + ".00 instead.\n"); //Goes all-in if the player has insufficient funds
                playerBets[handIndex] += HumanPlayer.PlaceBet(HumanPlayer.Bank);
            }
            playerHands[handIndex].AddCard(Deck.DealCard());
            frozenHands[handIndex] = true;
        }

        /* Take another card or stay on the current value.  This is also
         * called if the player refuses a double down to ensure that the
         * main game loop can advance properly */
        protected bool HitOrStay(CardList hand, int handIndex)
        {
            Console.WriteLine("(H)it or (S)tay on hand " + (handIndex + 1).ToString() + "?");
            char userInput = Console.ReadKey(true).KeyChar.ToString().ToUpper()[0]; //Stores the next key pressed as an uppercase char
            switch (userInput)
            {
                case 'H':
                    Console.WriteLine("Hit!");
                    hand.AddCard(Deck.DealCard()); //Adds a new card to the current hand
                    if (CheckHandScore(hand) >= 21)
                        frozenHands[handIndex] = true;
                    return true;
                    
                case 'S':
                    Console.WriteLine("Stay.");
                    frozenHands[handIndex] = true; //Makes sure the hand will be skipped on future iterations
                    return true;
                default:
                    break;
            }
            return false;
        }

        #endregion

        #region Hand Resolution

        // Allows the dealer to take an additional card if necessary
        protected void DealerHit()
        {
            Console.WriteLine("\nDealer Hits");
            dealerHand.AddCard(Deck.DealCard());
        }

        /* This method displays the final score for the dealer hand, then displays
         * the result of each player hand one at a time.  This can never be called
         * if all of a player's hands have been busted thus it only applies to */
        protected void ResolvePlayerHands()
        {
            ShowDealerScore();
            CasinoDoor.WaitForDisplay(); //A short delay before displaying the player's hands

            int dealerScore = CheckHandScore(dealerHand); //Stores the dealer's hand score

            //Player hands are resolved one at a time
            for (int handIndex = 0; handIndex < playerHands.Count; handIndex++)
            {
                int handScore = CheckHandScore(playerHands[handIndex]);

                Console.Write("Hand " + (handIndex + 1).ToString() + ": ");
                if ((handScore > dealerScore) || (handScore < dealerScore && dealerScore > 21)) //A player always wins these hands
                {
                    ResolveWinningHand(handIndex);
                }
                else if (handScore < dealerScore && dealerScore <= 21) //A player always loses this hand
                {
                    ResolveLosingHand(handIndex);
                }
                else //If the player's hand and the dealer both have the same hand value
                {
                    ResolveTiedHand(handIndex); //A player wins if this hand is a blackjack and the dealer's isn't, otherwise it is a push
                }
                CasinoDoor.RestoreDefaultColors();//The default colors are restored after each hand as each hand resolution may have unique coloring
                CasinoDoor.WaitForDisplay(); //Each hand is displayed with a slight delay before the next reveal
            }
        }

        /* This is a helper method for the resolution of player hands.  It displays
         * the dealer's score in one of 3 ways based on whether it us less than,
         * equal to, or greater than 21*/
        private void ShowDealerScore()
        {
            int dealerScore = CheckHandScore(dealerHand); //Stores the hand score for use

            Console.Write("\nDealer hand: "); //A label to identify that this is the dealer's hand
            if (dealerScore > 21)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("BUSTED"); //Busted hands are identified with red text on black background
            }
            else if (dealerScore == 21) //A dealer score of 21 has special rendering considerations
            {
                if (dealerHand.Cards.Count == 2) //Renders a dealer blackjack
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("BLACK"); //The first half of blackjack is displayed with red text on black
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("JACK"); //The second half of blackjack is displayed with black text on red
                }
                else //Renders a non-blackjack 21
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Score = 21"); //A non-blackjack 21 hand is displayed with green text on black
                }
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Score = " + dealerScore.ToString()); //A non-21 dealer hand is displayed with blue text on black
            }
            Console.WriteLine();
            CasinoDoor.RestoreDefaultColors(); //Default colors need to be restored after displaying the dealer's score
        }

        #endregion

        #region Win/Lose/Draw Helper Methods

        /* This resolves any hand in which the player has beat the dealer.  A player
         * beats the dealer if their hand score is larger than that of the dealers
         * without going over 21*/
        private void ResolveWinningHand(int handIndex)
        {
            float payout;
            if (playerHands[handIndex].Cards.Count == 2 && CheckHandScore(playerHands[handIndex]) == 21) //This signifies a blackjack
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("BLACK"); //Black is written with red text on black
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("JACK"); //Jack is written with black text on red

                CasinoDoor.RestoreDefaultColors();

                payout = ((playerBets[handIndex] * 1.5f) + playerBets[handIndex]); //A blackjack pays out at a 3:2 ratio
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("WIN"); //A standard winning hand is signified by green text on black

                payout = playerBets[handIndex] * 2; //A blackjack pays out at a 3:2 ratio
            }
            Console.WriteLine(" pays $" + payout + ".00");
            HumanPlayer.AddToBank(payout);
        }

        /* In a losing hand the player doesn't get any money added to their
         * bank*/
        private void ResolveLosingHand(int handIndex)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("LOSES"); //A losing hand is signified with red text on black
            Console.WriteLine(" $" + playerBets[handIndex].ToString() + ".00");
        }

        /* Resolves hands where the dealer and player have the same hand score. Players
         * can still win a tied hand if they have a blackjack*/
        private void ResolveTiedHand(int handIndex)
        {
            if (CheckHandScore(dealerHand) == 21)
            {
                if (playerHands[handIndex].Cards.Count == 2 && dealerHand.Cards.Count > 2)
                    ResolveWinningHand(handIndex); //A player blackjack beats a non-blackjack dealer 21
                else
                    ResolvePush(handIndex);
            }
            else
                ResolvePush(handIndex);
            CasinoDoor.RestoreDefaultColors();
        }

        //Called by ResolveTiedHand in the event of a push
        private void ResolvePush(int handIndex)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("PUSH");
            HumanPlayer.AddToBank(playerBets[handIndex]);
            Console.WriteLine(" reclaims $" + playerBets[handIndex].ToString() + ".00");
        }

        #endregion

        /* Gathers all the cards that have been dealt and places them back in the deck 
         * Also clears all of the relevant lists to allow another hand to be played*/
        public void GatherDeck()
        {
            foreach(Card card in dealerHand.Cards)
            {
                Deck.AddCard(card);
            }
            foreach (CardList hand in playerHands)
            {
                foreach (Card card in hand.Cards)
                    Deck.AddCard(card);
            }

            //All of these items need cleared in order to successfully play another hand
            playerBets.Clear();
            frozenHands.Clear();
            dealerHand.Cards.Clear();
            playerHands.Clear();
        }
    }
}
