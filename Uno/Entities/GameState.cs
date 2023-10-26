namespace Entities;

public class GameState
{
    public CardDeck GameDeck { get; set; } = new CardDeck();
    public CardDeck UsedDeck { get; set; } = new CardDeck();
    public int ActivePlayerNo { get; set; } = 0; 
    public List<Player> Players { get; set; } = new List<Player>();
    
    public int CurrentRoundNo { get; set; } = 0;
    
    public static int NumberOfCards = 112;

}