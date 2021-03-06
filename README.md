# ConsoleJack

This is a very simple blackjack game written for the Windows Command prompt. Some work would need to be done, but with some proper refactoring and additional code it could be expanded to support a variety of card games using some of the pieces that are already here.  The design is definitely not perfect but this was my first project after not coding for a few years. This game was a means to get comfortable with the C# and Visual Studio features again and demonstrates my ability to implement several key Object-Oriented concepts.  The project uses suit values in which requires unicode support.  If the suit values do not display properly try changing your console font, not every font supports them.  This project should build just fine in any modern-ish version of Visual Studio, there are no outside libraries and I don't think there are any cutting-edge features.  It should run fine just about anywhere.

##Object Oriented Programming##
This project uses several classes.  Card objects are organized into a list within a Deck object.  The deck object has methods that allow the deck to be shuffled.  The shuffle process uses the Fisher-Yates shuffle algorithm and it works similar to sorting a list of objects.

##Programming Constructs##

There are several standard programming constructs in this project including switches (Card, DeckOfCards), for/foreach loops (DeckOfCards, BlackJackTable, BlackJackRules), while loops (BlackJackTable), if/else statements (BlackJackRules), boolean checks including some from functions with return values (BlackJackRules.)

##String Manipulation##

Basically the entire program implements string manipulation techniques due to the console-based interface.  There are escape sequences for line breaks and even some to display suit values in the console.  If suit values are displaying incorrectly try changing the font in your console, not all fonts support the full range of UTF-8 characters that are used in this project.

##Array/List Manipulation##

There are several examples of this, one of the decent ones is in the CardList class.  This class is semi-generic, it stores a variable length list that can have cards added/removed as needed.  DeckOfCards inherits from CardList to create a standard 52-card deck, and the player/dealer hands are just standard instantiations of the CardList class.  The DeckOfCards class implements the Fisher-Yates algorithm to shuffle the deck between hands.

##Inheritance##

The above-mentioned DeckOfCards is a simple example of inheritance.  The BlackJackTable class is a larger example, it is primarily responsible for the visual aspects of the game (drawing to the console) and managing the game loop.  BlackJackTable inherits from BlackJackRules in order to access necessary deck, dealer functions, and any active hands of cards.  The underlying BlackJackRules class contains helper functions to verify game logic, score hands, and handle bets

##Interfaces##

There are a pair of simple interfaces in this project.  They were probably somewhat unnecessary but I wanted to make sure I knew how to implement them.  IGameTable and IDealer are the two examples here.  The idea behind IGameTable and IDealer is that games other than BlackJack could be later added to this project with the interfaces providing a template for certain functionality (dealers for different games might deal a different number of cards for example, and different game tables might host different numbers of players.
