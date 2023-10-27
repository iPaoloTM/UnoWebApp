using System.Net;
using System.Text.Json;
using Entities;

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

            currPlayer = Game.Players[Game.ActivePlayerNo];
            DrawMenu();

            //Print card information of current player
            ShowHand();

            Console.WriteLine();

            // Ask the player for his choice
            PlayerPrompt();
        } while (true); //Change condition later

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
        string? choice;
        //bool turnOver = false; Maybe?
        do //While the player hasn't skipped...
        {
            Console.WriteLine("Choose an action: ");
            Console.WriteLine("1. Play a card ");
            Console.WriteLine("2. Draw from deck ");
            Console.WriteLine("3. Skip ");
            choice = Console.ReadLine();
            switch (choice)
            {
                // Try to play a card
                case "1":
                    ShowLastCard();
                    ShowHand();
                    var maxCards = currPlayer.HandCards.Count;
                    Console.WriteLine("Choose a card from 1 to " + maxCards + ": ");
                    var chosenCard = Console.ReadLine();
                    if (int.TryParse(chosenCard, out var chosenInt) && chosenInt > 0 && chosenInt < maxCards)
                    {
                        var playingCard = currPlayer.HandCards[chosenInt - 1];
                        var movePlay =
                            new PlayerMove(currPlayer, EPlayerAction.PlayCard, playingCard);
                        //Check if the move is valid? Will edit when method is clearly defined
                        //if ... { Handle accepted move...}
                        Game.HandlePlayerAction(currPlayer, movePlay);
                    }
                    break;
                case "2":
                    //The player wants to draw a card
                    //Card should be null here??
                    var moveDraw = new PlayerMove(currPlayer, EPlayerAction.Draw, null);
                    //Check if the player can NOT play a card to validate the draw action?
                    Game.HandlePlayerAction(currPlayer, moveDraw);
                    break;
                case "3":
                    //Player wants to skip to the next player?
                    var moveSkip = new PlayerMove(currPlayer, EPlayerAction.NextPlayer, null);
                    Game.HandlePlayerAction(currPlayer, moveSkip);
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        } while (choice != "3");
        
        
        //Assume the turn is over, prompt the user if they want to say something
        //Should say UNO if they have 1 card left
        Console.WriteLine("Do you want to say something?");
        var screamingPlayer = Console.ReadLine();
        
        //Check if player has said uno when his turn ends and has 1 card left
        if (screamingPlayer != null && screamingPlayer.ToLower().Trim() != "uno" && 
            currPlayer.HandCards.Count == 1)
        {
            // Need to add some sort of UNO FLAG to GameEngine.cs, enable it here
            //Game.ForgotUno(PlayerNumber?) should remember who forgot it
        }
        
        //Someone else says uno after the player forgot to say it
        else if (screamingPlayer!= null  && screamingPlayer.ToLower().Trim() == "uno")
            //&& Game.UnoFlag == 1
        {
            // Punish player that has 1 card who didnt say Uno, remove uno flag
            //Game.Players[PlayerNumber] take two cards, cant find a function to do it
            //Game.UnoFlag = 0;
        }
        
        //De-activate the uno flag when the player who forgot gets his turn again
        //Think how to do it?

        // Pass turn to next player 
        Game.ActivePlayerNo++;
        if (Game.ActivePlayerNo >= Game.Players.Count) Game.ActivePlayerNo = 0;
    }

    public void ShowHand()
    {
        Console.WriteLine("\nYour Hand:");
        int i = 0;
        //Add colored text in the future
        foreach (Card c in currPlayer.HandCards)
        {
            if (c is NumericCard)
            {
                NumericCard nc = (NumericCard)c;
                Console.Write(i+1 + ") " + nc.Color + " " + nc.Number + ", ");
            }
            else if (c is SpecialCard)
            {
                SpecialCard nc = (SpecialCard)c;
                Console.Write(i+1 + ") " + nc.Color + " " + nc.Effect + ", ");
            }

            i++;
        }
    }

    public void ShowLastCard()
    {
        var lastCard = Game.UsedDeck.First();
        switch (lastCard)
        {
            case NumericCard numCard:
                Console.WriteLine("Last played card: " + numCard.Color + " " + numCard.Number);
                break;
            case SpecialCard speCard:
                Console.WriteLine("Last played card: " + speCard.Color + " " + speCard.Effect);
                break;
        }
    }

    public void DrawMenu()
    {
        //Display whose turn it is
        Console.WriteLine("Player " + currPlayer.Nickname + "'s turn");
        Console.WriteLine("================================================");

        //Print the number of cards each player has
        foreach (Player plyr in Game.Players)
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