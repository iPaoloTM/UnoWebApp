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
        Deck.AddRange(drawnCard); //what if CardDeck will be empty? call function that will create new shuffled deck
                                  //or shouldnt allow to return null CardDeck? 
        
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

    private bool HasMatch(Card card)
    {
        //refactor of Cards.cs and SpecialCards.cs and NumericCard.cs or create function for both of them.
        // here is also matching by color or number(value).
        //possibility control the match just by color input and check matching colors
        return Deck.Any(x => x.Color == card.Color || x.Number == card.Number|| x.Color == EColors.Black);
    }

    private PlayerTurn PlayMatchingCard(Card currentDiscard)
    {
        //TODO - Michal
    }
    
    
}