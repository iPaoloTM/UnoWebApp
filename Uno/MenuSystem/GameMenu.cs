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

    public void Run()
    {
        Console.Clear();
        
        Player currPlayer = Game.Players[Game.ActivePlayerNo];
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
        Console.WriteLine(Game.UsedDeck.Cards.Count);
        Console.ReadLine();
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
        
        
        Console.WriteLine("\nYour Hand:");
        //Print card information of current player
        foreach (Card c in currPlayer.HandCards)
        {
            if (c is NumericCard)
            {
                NumericCard nc = (NumericCard)c;
                Console.Write(nc.Color+" "+nc.Number +", ");
            } else if (c is SpecialCard)
            {
                SpecialCard nc = (SpecialCard)c;
                Console.Write(nc.Color+" "+nc.Effect +", ");
            }
        }
        
        Console.WriteLine();
        
        // Ask the player for input
        Console.WriteLine("Choose an action: ");
        Console.WriteLine("1. Play a card ");
        Console.WriteLine("2. Draw from deck ");
        Console.WriteLine("3. Skip ");
        string? choice = Console.ReadLine();

        
        //Test json
        var jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            AllowTrailingCommas = true

        };
        
        Game.SaveGameState();
        Console.WriteLine(JsonSerializer.Serialize(this.Game.GameState,jsonOptions));
        Console.ReadLine();


        //Game.HandlePlayerAction(currPlayer, decision?)


    }
    
}