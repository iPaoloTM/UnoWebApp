using Entities;

namespace MenuSystem;

using UnoEngine;

public class GameMenu
{
    public GameMenu(UnoEngine game)
    {
        this.game = game;
        
    }

    public UnoEngine game { get; set; }

    public void Draw()
    {
        //Display whose turn it is
        Console.WriteLine("Player nr. " + game.State.ActivePlayerNo + "'s turn");
        Console.WriteLine("================================================");

        //Print the number of cards each player has
        foreach (Player plyr in game.Players)
        {
            Console.Write(plyr.Nickname + " - ");
            foreach (Card c in plyr.Deck)
            {
                Console.Write("#");
            }
            Console.WriteLine();
        }
        
        //TODO: Print info about current player cards
        //How to access the player who is playing right now?
        //  ActivePlayerNo --> Type INT --> Cant get cards
        //foreach (Card c in game.State.)
        
        Console.WriteLine();
        
        //PlayGame function goes here?
    }
    
}