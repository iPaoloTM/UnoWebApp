namespace Domain;

public class CardsInPlayOnTheTable
{
    public List<GameCard> ReceivedCards { get; set; } = new List<GameCard>();
    public List<GameCard> KillCards { get; set; } = new List<GameCard>();

    public void Reset()
    {
        ReceivedCards = new List<GameCard>();
        KillCards = new List<GameCard>();
    }

    public bool CanBeForwarded()
    {
        if (KillCards.Count == 0) return true;
        if (ReceivedCards.Count == 1) return true;
        if (ReceivedCards.Count ==
            ReceivedCards.Count(card => ReceivedCards.First().CardValue == card.CardValue)) return true;
        return false;
    }
}