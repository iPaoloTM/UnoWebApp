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
        //Display whose turn it is
        Console.WriteLine("Player nr. " + Game.State.ActivePlayerNo + "'s turn");
        Console.WriteLine("================================================");

        //Print the number of cards each player has
        foreach (Player plyr in Game.Players)
        {
            Console.Write(plyr.Nickname + " - ");
            foreach (Card c in plyr.Deck)
            {
                //Print a # for each card
                Console.Write("#");
            }
            Console.WriteLine();
        }
        
        
        //Print card information of current player
        foreach (Card c in Game.Players[Game.State.ActivePlayerNo].Deck)
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
        
        //PlayGame function goes here
    }
    
}