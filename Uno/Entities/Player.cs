// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using Entities;

public class Player
{
    public string Nickname { get; set; } = default!;
    public EPlayerType PlayerTipe { get; set; }

    public List<Card> Deck { get; set; } = new List<Card>();

    public Player()
    {
        Deck = new List<Card>();
    }

    public PlayerTurn PlayTurn()
    {
        //TODO - Kata
    }

    public PlayerTurn DrawCard(PlayerTurn previousTurn, CardDeck cardDeck)
    { 
        PlayerTurn turn = new PlayerTurn();
        var drawnCard = cardDeck.Draw(1);
        Deck.AddRange(drawnCard); //what if CardDeck will be empty? call function that will create new shuffled deck.
        
        if (HasMatch(previousTurn.Card))
        {
        turn = PlayMatchingCard(previousTurn.Card);
        turn.Result = TurnResult.ForceDrawPlay; //enum TurnResult was created by Kata in katbranch
        }
        else
        {
        turn.Result = TurnResult.ForceDraw; //enum TurnResult was created by Kata in katbranch
        turn.Card = previousTurn.Card;
        }
        
        return turn;
    }
    
    public void DisplayTurn()
    {
        //TODO - Kata
    }
    
    public PlayerTurn ProccesAttack()
    {
        //TODO - Kata
    }

    private bool hasMatch()
    {
        //TODO - Michal

    }

    private PlayerTurn PlayMatchingCard(Card currentDiscard)
    {
        //TODO - Michal
    }
    
    
}