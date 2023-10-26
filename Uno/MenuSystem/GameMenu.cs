using Entities;

namespace MenuSystem;

using UnoEngine;

public class GameMenu
{
    public GameMenu(UnoEngine game)
    {
        this.Game = game;
        
    }

    public UnoEngine Game { get; set; }

    public void Draw()
    {
        Console.Clear();
        
        Player currPlayer = Game.Players[Game.GameState.ActivePlayerNo];
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
        
        //Game.HandlePlayerAction(currPlayer, decision?)
        
        
    }
    
}