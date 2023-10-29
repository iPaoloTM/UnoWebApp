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
    private Player CurrPlayer { get; set; } = default!;

    public void Run()
    {
        do
        {
            Console.Clear();
            Game.NewTurn();
            
            CurrPlayer = Game.State.Players[Game.State.ActivePlayerNo];
            Console.WriteLine("--- " + CurrPlayer.Nickname + "'S TURN ---");
            Console.ReadLine();
            Console.Clear();
            
            DrawMenu();

            //Print card information of current player
            ShowHand();
            
            // Ask the player for his choice
            PlayerPrompt();
        } while (!Game.State.GameOver);

        Console.Clear();
        Console.WriteLine("> G A M E  O V E R <");
        Console.WriteLine("Congratulations!");
        Console.WriteLine("Player: " + CurrPlayer.Nickname + " has won the game!");
        Console.ReadLine();
    }


    //ASK THE PLAYER FOR HIS ACTIONS IN THIS FUNCTION!!
    public void PlayerPrompt()
    {
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
                    if (Game.turnOver)
                    {
                        Console.WriteLine("You already acted this turn!");
                        Console.ReadLine();
                    }
                    else
                    {
                        var maxCards = CurrPlayer.HandCards.Count;
                        Console.WriteLine("Choose a card from 1 to " + maxCards + ": ");
                        var chosenCard = Console.ReadLine();
                        if (int.TryParse(chosenCard, out var chosenInt) && chosenInt > 0 && chosenInt <= maxCards)
                        {
                            var playingCard = CurrPlayer.HandCards[chosenInt - 1];
                            var movePlay =
                                new PlayerMove(CurrPlayer, EPlayerAction.PlayCard, playingCard);
                            success = Game.HandlePlayerAction(movePlay);
                            if (success == 2) //Player needs to choose a color
                            {
                                Console.WriteLine("Choose a new color: ");
                                Console.WriteLine("1) Red");
                                Console.WriteLine("2) Blue");
                                Console.WriteLine("3) Yellow");
                                Console.WriteLine("4) Green");
                                string? color = Console.ReadLine();
                                Game.SetColorInPlay(int.Parse(color));
                            }
                            else if (success != 1 && success != 4)
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
                    if (Game.turnOver)
                    {
                        Console.WriteLine("You already acted this turn!");
                        Console.ReadLine();
                    }
                    else if (!Game.canDraw)
                    {
                        Console.WriteLine("You can play one of your cards!");
                        Console.ReadLine();
                    }
                    else
                    {
                        //The player wants to draw a card
                        //Card should be null here??
                        var moveDraw = new PlayerMove(CurrPlayer, EPlayerAction.Draw, null);
                        success = Game.HandlePlayerAction(moveDraw);
                        if (success != 3 && success != 1)
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
                    Game.HandleUnoShouting(CurrPlayer, screamingPlayer);
                    Game.HandleUnoReporting(screamingPlayer);
                    Console.Clear();
                    DrawMenu();
                    ShowHand();
                    break;
                //PLayer wants to end his turn 
                case "4":
                    //Only end turn if the player has drawn or played
                    var moveSkip = new PlayerMove(CurrPlayer, EPlayerAction.NextPlayer, null);
                    Game.HandlePlayerAction(moveSkip);
                    if(!Game.turnOver)
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
        } while (!Game.endTurn);
    }

    public void ShowHand()
    {
        Console.WriteLine("Your Hand:");
        int i = 0;
        //Add colored text in the future
        foreach (Card c in CurrPlayer.HandCards)
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
        Console.WriteLine("Player " + CurrPlayer.Nickname + "'s turn");
        Console.WriteLine("================================================");

        int a = 0;
        //Print the number of cards each player has
        foreach (Player plyr in Game.State.Players)
        {
            Console.Write(a+1 +". " + plyr.Nickname + " - ");
            foreach (Card c in plyr.HandCards)
            {
                //Print a # for each card
                Console.Write("#");
            }

            Console.WriteLine();
            a++;
        }

        //Show the last card of the used deck
        ShowLastCard();
    }
}