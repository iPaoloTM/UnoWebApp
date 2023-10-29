using System.Net;
using System.Text.Json;
using Entities;
using UnoEngine;

namespace MenuSystem;

using UnoEngine;

public class GameMenu
{
    public GameMenu(GameEngine game)
    {
        this.Game = game;
    }

    public GameEngine Game { get; set; }
    private Player currPlayer { get; set; } = default!;

    public void Run()
    {
        do
        {
            Console.Clear();

            currPlayer = Game.State.Players[Game.State.ActivePlayerNo];
            DrawMenu();

            //Print card information of current player
            ShowHand();

            // Ask the player for his choice
            PlayerPrompt();
        } while (!Game.State.GameOver);

        Console.Clear();
        Console.WriteLine("> G A M E  O V E R <");
        Console.WriteLine("Congratulations!");
        Console.WriteLine("Player: " + currPlayer.Nickname + " has won the game!");
        Console.ReadLine();

        /*

        //Test json for serializing without converting everything
        //Easy with newtonsoft but CANT USE IT IN THE COURSE :(
        //Misses getting the attributes from SpecialCard and NumericCard, everything else works

        var jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            AllowTrailingCommas = true

        };

        Game.SaveGameState();
        Console.WriteLine(JsonSerializer.Serialize(this.Game.GameState,jsonOptions));
        Console.ReadLine();
        */
    }


    //ASK THE PLAYER FOR HIS ACTIONS IN THIS FUNCTION!!
    public void PlayerPrompt()
    {
        var turnOver = false;
        var canDraw = true;
        var endTurn = false;
        string? screamingPlayer = null;
        do //While the player hasn't skipped...
        {
            Console.WriteLine("Choose an action: ");
            Console.WriteLine("1. Play a card ");
            Console.WriteLine("2. Draw from deck ");
            Console.WriteLine("3. Say something");
            Console.WriteLine("4. Skip ");
            var choice = Console.ReadLine();
            int success = 0;
            switch (choice)
            {
                // Try to play a card
                case "1":
                    if (turnOver)
                    {
                        Console.WriteLine("You already acted this turn!");
                        Console.ReadLine();
                    }
                    else
                    {
                        var maxCards = currPlayer.HandCards.Count;
                        Console.WriteLine("Choose a card from 1 to " + maxCards + ": ");
                        var chosenCard = Console.ReadLine();
                        if (int.TryParse(chosenCard, out var chosenInt) && chosenInt > 0 && chosenInt <= maxCards)
                        {
                            var playingCard = currPlayer.HandCards[chosenInt - 1];
                            var movePlay =
                                new PlayerMove(currPlayer, EPlayerAction.PlayCard, playingCard);
                            //Check if the move is valid? Will edit when method is clearly defined
                            //if ... { Handle accepted move...}
                            success = Game.HandlePlayerAction(movePlay);
                            if (success == 1) turnOver = true;
                            else if (success == 2) //Player needs to choose a color
                            {
                                Console.WriteLine("Choose a new color: ");
                                Console.WriteLine("1) Red");
                                Console.WriteLine("2) Blue");
                                Console.WriteLine("3) Yellow");
                                Console.WriteLine("4) Green");
                                string? color = Console.ReadLine();
                                Game.SetColorInPlay(int.Parse(color));
                                turnOver = true;
                            }
                            else if (success == 4) //Player has played his last card, the game is over.
                            {
                                turnOver = true;
                                endTurn = true;
                            }
                            else
                            {
                                Console.WriteLine("Can't play the selected card!");
                                Console.ReadLine();
                            }
                        }
                    }

                    Console.Clear();
                    DrawMenu();
                    ShowHand();
                    break;
                //Try to draw a card from the game deck
                case "2":
                    if (turnOver)
                    {
                        Console.WriteLine("You already acted this turn!");
                        Console.ReadLine();
                    }
                    else if (!canDraw)
                    {
                        Console.WriteLine("You can play one of your cards!");
                        Console.ReadLine();
                    }
                    else
                    {
                        //The player wants to draw a card
                        //Card should be null here??
                        var moveDraw = new PlayerMove(currPlayer, EPlayerAction.Draw, null);
                        success = Game.HandlePlayerAction(moveDraw);
                        if (success == 1)
                        {
                            turnOver = true;
                            canDraw = false;
                        }
                        else if (success == 3)
                        {
                            canDraw = false;
                        }
                        else
                        {
                            Console.WriteLine("You can play one of your cards!");
                            Console.ReadLine();
                        }
                    }

                    Console.Clear();
                    DrawMenu();
                    ShowHand();
                    break;
                //Say something (uno)
                case "3":
                    Console.WriteLine("What do you want to say?");
                    screamingPlayer = Console.ReadLine();
                    Game.HandleUnoShouting(currPlayer, screamingPlayer);
                    Game.HandleUnoReporting(screamingPlayer);
                    break;
                //PLayer wants to end his turn 
                case "4":
                    //Only end turn if the player has drawn or played
                    var moveSkip = new PlayerMove(currPlayer, EPlayerAction.NextPlayer, null);
                    Game.HandlePlayerAction(moveSkip);
                    if (turnOver)
                    {
                        endTurn = true;
                    }
                    else
                    {
                        Console.WriteLine("Can't end turn without doing an action");
                        Console.ReadLine();
                        Console.Clear();
                        DrawMenu();
                        ShowHand();
                    }

                    break;

                default:
                    Console.WriteLine("Invalid option");
                    Console.ReadLine();
                    Console.Clear();
                    DrawMenu();
                    ShowHand();
                    break;
            }
        } while (!endTurn);
    }

    public void ShowHand()
    {
        Console.WriteLine("Your Hand:");
        int i = 0;
        //Add colored text in the future
        foreach (Card c in currPlayer.HandCards)
        {
            if (c is NumericCard)
            {
                NumericCard nc = (NumericCard)c;
                Console.Write(i + 1 + ") " + nc.Color + " " + nc.Number + ", ");
            }
            else if (c is SpecialCard)
            {
                SpecialCard nc = (SpecialCard)c;
                Console.Write(i + 1 + ") " + nc.Color + " " + nc.Effect + ", ");
            }

            i++;
        }

        Console.WriteLine();
    }

    public void ShowLastCard()
    {
        var lastCard = Game.State.UsedDeck.First();
        switch (lastCard)
        {
            case NumericCard numCard:
                Console.WriteLine("Last played card: " + numCard.Color + " " + numCard.Number);
                break;
            case SpecialCard speCard:
                Console.WriteLine("Last played card: " + speCard.Color + " " + speCard.Effect);
                break;
        }

        if (Game.State.UsedDeck.Cards.First().Color == EColors.Black)
        {
            Console.WriteLine("Chosen color: " + Game.State.ColorInPlay);
        }

        Console.WriteLine();
    }

    public void DrawMenu()
    {
        //Display whose turn it is
        Console.WriteLine("Player " + currPlayer.Nickname + "'s turn");
        Console.WriteLine("================================================");

        //Print the number of cards each player has
        foreach (Player plyr in Game.State.Players)
        {
            Console.Write(plyr.Nickname + " - ");
            foreach (Card c in plyr.HandCards)
            {
                //Print a # for each card
                Console.Write("#");
            }

            Console.WriteLine();
        }

        //Show the last card of the used deck
        ShowLastCard();
    }
}