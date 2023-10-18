namespace Entities;

public class GameState
{
    public List<Card> GameDeck { get; set; } = new List<Card>();
    public List<Card> UsedDEck { get; set; } = new List<Card>();
    public int ActivePlayerNo { get; set; } = 0; // TODO: how to choose the starter?
    public List<Player> Players { get; set; } = new List<Player>();
        
}