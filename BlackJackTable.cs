using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleJack
{
    /* The BlackJackTable inherits from the BlackJackRules class.
     * The table is primarily responsible for displaying player
     * selections and advancing the game loop while the rules class
     * gathers player input and handles scoring and the checking of hands*/
    class BlackJackTable : BlackJackRules, IGameTable
    {        
        public BlackJackTable(Player currentPlayer) : base()
        {
            Deck = new DeckOfCards(); //Initializes the deck
            HumanPlayer = currentPlayer; //The player starts with 100 dollars
            
            PlayGame();
        }

        #region Main Game Loop

        //PlayGame initializes the main game loop.
        public void PlayGame()
        {
            bool gameInProgress = true; //This value will remain true until a player decides to stop playing

            while (gameInProgress)
            {
                if (PlayHand() == false)
                    gameInProgress = false;
            }
            Console.WriteLine("Press any key to exit the Dot Net Coresino");
            Console.ReadKey();
        }

        /* PlayHand initializes the loop for a single hand of BlackJack.  Returns
         * true or false based on whether or not the player wants to keep playing
         * at the end of a hand*/
        private bool PlayHand()
        {
            bool handInProgress = true;  //This variable tracks whether a hand is complete
            bool dealerHandRevealed = false;
            while (handInProgress)
            {
                DisplayTable(dealerHandRevealed);
                if (playerHands.Count == 0) //If the player is not currently holding any cards
                {
                    GetWagerAndDeal();
                }
                else //If the player has at least one hand
                {
                    Console.WriteLine();
                    if (AllHandsFrozen() == true) //If all the hands are busted, doubled down, or stayed
                    {
                        if (AllHandsBusted() == false)
                        {
                            dealerHandRevealed = true;
                            DisplayTable(dealerHandRevealed);
                            if (CheckHandScore(dealerHand) < 17)
                            {
                                CasinoDoor.WaitForDisplay();
                                DealerHit();
                            }
                            else
                            {
                                dealerHandRevealed = true;
                                ResolvePlayerHands();
                                handInProgress = false;
                            }
                        }
                        else
                        {
                            handInProgress = false;
                        }

                        Console.WriteLine(); //Spacing for readability
                        CasinoDoor.WaitForDisplay();
                    }
                    else
                    {
                        OfferPlayerOptions();
                        CasinoDoor.WaitForDisplay();
                    }
                }
            }
            if (HumanPlayer.Bank > 0)
                return PlayAnotherHand(true);
            else
            {
                Console.WriteLine("You went bankrupt! Time to go home.");
                return false;
            }
        }

        //This is called by PlayGame when the player has no cards
        private void GetWagerAndDeal()
        {
            if (InitialWagerPrompt() == true)
            {
                Console.WriteLine();
                DealHands();
                CasinoDoor.WaitForDisplay();
            }
            else
            {
                DisplayError("Invalid wager, try again");
            }
        }

        //This is called by PlayGame when there is one or more hands needing attention from the user
        private void OfferPlayerOptions()
        {
            for (int handIndex = 0; handIndex < playerHands.Count; handIndex++) //Iterates over the currently present hands
            {
                if (frozenHands[handIndex] == false) //Checks to see if this hand has been frozen
                {
                    int cardsInHand = playerHands[handIndex].Cards.Count;

                    if (cardsInHand == 2 && HumanPlayer.Bank > 0) //If the player's hand has 2 cards and the player has funds remaining
                    {
                        if (CheckBlackJack(playerHands[handIndex], handIndex) == true) //Checks for a blackjack
                        {
                            AnnounceBlackJack(handIndex);
                        }
                        else if (CheckSplit(playerHands[handIndex]) == true) //Checks to see if the cards are a pair
                        {
                            if (SplitHandPrompt(playerHands[handIndex], handIndex) == true) //Prompts the player to split the hand and validates input
                            {
                                CasinoDoor.WaitForDisplay();
                                break;
                            }
                            else //If the user presses a wrong key when attempting to split a hand
                            {
                                DisplayError("Input for split attempt invalid, please try again."); //Occurs when the player's input is invalid
                                break;
                            }
                        }
                        else
                        {
                            if (DoubleDownPrompt(playerHands[handIndex], handIndex) == true) //Prompts the player to double down and verifies input
                            {
                                CasinoDoor.WaitForDisplay();
                                break;
                            }
                            else //If the user presses an incorrect key when attempting to double down
                            {
                                DisplayError("Input for double down attempt invalid, please try again."); //Occurs when the player's input is invalid
                                break;
                            }
                        }
                        
                    }
                    else if (cardsInHand == 2 && HumanPlayer.Bank <= 0) //If the player has 2 cards but does not have any funds remaining
                    {
                        if (CheckBlackJack(playerHands[handIndex], handIndex) == true) //Checks for a blackjack
                        {
                            AnnounceBlackJack(handIndex);
                        }
                        else if (CheckSplit(playerHands[handIndex]) == true) //If the hand is not a blackjack but is a face-value pair
                        {
                            Console.WriteLine("Insufficient funds to split hand.");
                            CasinoDoor.WaitForDisplay();
                        }
                        else
                        {
                            Console.WriteLine("Insufficient funds to double down on hand."); //If the hand is neither blackjack nor a pair
                            CasinoDoor.WaitForDisplay();
                            HitOrStay(playerHands[handIndex], handIndex); //This ensures that the split/double down checks won't loop endlessly
                        }
                    }
                    else //If this hand has more than 2 cards in it
                    {
                        if (CheckHandScore(playerHands[handIndex]) < 21)
                            HitOrStay(playerHands[handIndex], handIndex);  //Prompts the player to hit or stay
                        break;
                    }
                }
            }
        }

        #endregion

        #region Table Display

        //Draws the score and the hands of cards
        public void DisplayTable(bool allHandsRevealed)
        {
            Console.Clear(); //Clears the console before re-drawing all game elements

            //The name of the current game is displayed in a stylized format
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkRed;            
            Console.Write("Console");
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Jack\n");            

            //Bank and wager values are displayed in green text on black
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Remaining Funds: " + HumanPlayer.Bank.ToString("c2"));
            DisplayBets();
            CasinoDoor.RestoreDefaultColors();
            DisplayDealerHand(allHandsRevealed);
            DisplayPlayerHands();
        }

        //A helper method to split DisplayTable into smaller parts
        private void DisplayBets()
        {
            for (int currentBet = 0; currentBet < playerBets.Count; currentBet++)
            {
                Console.WriteLine("Hand " + (currentBet + 1).ToString() + " wager: " + playerBets[currentBet].ToString("c2"));
            }
            Console.WriteLine();
        }

        //A helper method to split DisplayTable into smaller parts
        private void DisplayDealerHand(bool holeCardRevealed)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Dealer Hand"); //The dealer's hand label is displayed in blue on black
            CasinoDoor.RestoreDefaultColors();
            int cardsInHand = dealerHand.Cards.Count;
            if (cardsInHand > 0)
            {
                for (int card = 0; card < cardsInHand; card++)
                {
                    if (!holeCardRevealed && card == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("?");
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("? ");
                        CasinoDoor.RestoreDefaultColors();
                    }
                    else
                        dealerHand.Cards[card].PrintCardInfo();
                }
            }
            Console.WriteLine();
        }

        //A helper method to split DisplayTable into smaller parts
        private void DisplayPlayerHands()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Player Hand(s)"); //The player's hand label is displayed in blue on black

            CasinoDoor.RestoreDefaultColors();

            int numberofHands = playerHands.Count;
            if (numberofHands > 0)
            {
                for (int handIndex = 0; handIndex < numberofHands; handIndex++)
                {
                    Console.Write((handIndex + 1).ToString() + ": ");

                    if (CheckHandScore(playerHands[handIndex]) > 21)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("BUST");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        for (int cardIndex = 0; cardIndex < playerHands[handIndex].Cards.Count; cardIndex++)
                        {
                            playerHands[handIndex].Cards[cardIndex].PrintCardInfo();
                        }
                        Console.WriteLine("= " + CheckHandScore(playerHands[handIndex]).ToString());
                    }
                }
            }
            else
                Console.WriteLine(); //Spacing for readability
        }

        //If the player makes an incorrect selection the error is displayed before they are re-prompted
        public void DisplayError(string error)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(error);
            CasinoDoor.RestoreDefaultColors();
            CasinoDoor.WaitForDisplay();
        }

        #endregion

        #region Helper Methods

        //A helper method that informs the player that they have hit a blackjack
        public void AnnounceBlackJack(int handIndex)
        {
            Console.Write("Hand " + (handIndex + 1) + ": ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("Black");
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Jack\n");
            CasinoDoor.RestoreDefaultColors();
        }

        //Returns true or false based on whether or not the player wants to keep playing
        private bool PlayAnotherHand(bool firstRun)
        {
            if (firstRun)
            {
                Console.WriteLine("Cards will be returned to the dealer.");
                GatherDeck();
            }

            Console.WriteLine("Play another hand? (Y/N): ");
            char playerInput = Console.ReadKey(true).KeyChar.ToString().ToUpper()[0];

            if (playerInput == 'Y')
            {
                return true;
            }
            else if (playerInput == 'N')
            {
                return false;
            }
            else
            {
                Console.WriteLine("Invalid selection, try again.");
                return PlayAnotherHand(false); //Calls itself recursively for invalid inputs
            }
        }

        #endregion
    }
}
