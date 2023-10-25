namespace Entities;

public class GameState
{
    public CardHand GameHand { get; set; } = new CardHand();
    public CardHand UsedHand { get; set; } = new CardHand();
    public int ActivePlayerNo { get; set; } = 0; 
    public List<Player> Players { get; set; } = new List<Player>();
    
    public int CurrentRoundNo { get; set; } = 0;
    
    public static int NumberOfCards = 112;

}