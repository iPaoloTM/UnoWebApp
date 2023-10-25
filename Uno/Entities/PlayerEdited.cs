using System.Diagnostics;

namespace Entities;

public class PlayerEdited
{
    public string Nickname { get; }
    public int Position { get; set; } = default!;
    public EPlayerType PlayerType { get; set; }

    public List<Card> HandCards { get; set; } = new List<Card>();

    public PlayerEdited(string nickname)
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
        gameEngine.HandlePlayerAction(this, PlayerAction.PlayCard, card);
    }

    public void Draw()
    {
        gameEngine.HandlePlayerAction(this, PlayerAction.Draw, null);
    }
    
    public void NextPlayer()
    {
        gameEngine.HandlePlayerAction(this, PlayerAction.NextPlayer, null);
    }

    public void Shout()
    {
        gameEngine.HandlePlayerAction(this, PlayerAction.Shout, null);
    }
}