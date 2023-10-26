using UnoEngine;

namespace Entities;


public class Player
{
    public string Nickname { get; }
    public int Position { get; set; } = default!;
    public EPlayerType PlayerType { get; set; }
    public List<Card> HandCards { get; set; } = new List<Card>();
    public EPlayerAction? PreviousAction { get; set; }
    
    UnoEngine unoEngine = UnoEngine.GetInstance();
    
    public Player(string nickname)
    {
        Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname), "Nickname cannot be null.");
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
    
    public void PlayCard(Card card)
    {
        unoEngine.HandlePlayerAction(this, EPlayerAction.PlayCard, card);
    }

    public void Draw()
    {
        unoEngine.HandlePlayerAction(this, EPlayerAction.Draw, null);
    }
    
    public void NextPlayer()
    {
        unoEngine.HandlePlayerAction(this, EPlayerAction.NextPlayer, null);
    }

    public void SaySomething()
    {
        unoEngine.HandlePlayerAction(this, EPlayerAction.SaySomething, null);
    }
}