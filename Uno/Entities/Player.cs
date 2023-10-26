
using System.Net.NetworkInformation;

namespace Entities;


public class Player
{
    public string Nickname { get; }
    public int Position { get; set; } = default!;
    public EPlayerType PlayerType { get; set; }
    public List<Card> HandCards { get; set; } = new List<Card>();
    public PlayerMove PreviousPlayerMove;
    
    
    public Player(string nickname, PlayerMove previousTurn)
    {
        Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname), "Nickname cannot be null.");
        PreviousPlayerMove = previousTurn;
        HandCards = new List<Card>();
    }
    
    public void AddCard(Card card)
    {
        HandCards.Add(card);
    }
    
    public void TakeCard(Card card)
    {
        if (card == null)
        {
            throw new ArgumentNullException(nameof(card), "Card cannot be null");
        }

        if (!HandCards.Contains(card))
        {
            throw new ArgumentException("The specified card is not in the player's hand", nameof(card));
        }

        HandCards.Remove(card);
    }
    
    public PlayerMove PlayCard(Card card)
    {
        var playerMove = new PlayerMove(this, EPlayerAction.PlayCard, null);
        return playerMove; 
    }

    public PlayerMove Draw( )
    {
        var playerMove = new PlayerMove(this, EPlayerAction.Draw, null);
        return playerMove;
    }

    public PlayerMove NextPlayer()
    {
        var playerMove = new PlayerMove(this, EPlayerAction.NextPlayer, null);
        return playerMove;
    }

    public PlayerMove SaySomething()
    {
        var playerMove = new PlayerMove(this, EPlayerAction.SaySomething, null);
        return playerMove;
    }
}