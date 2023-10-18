namespace Domain;

public class GameState
{
    public List<GameCard> DeckOfCardsInPlay { get; set; } = new List<GameCard>();
    public List<GameCard> DeckOfCardsGraveyard { get; set; } = new List<GameCard>();
    public CardsInPlayOnTheTable CardsInPlayOnTheTable { get; set; } = new CardsInPlayOnTheTable();
    public GameCard? TrumpCard { get; set; }
    public int ActivePlayerNo { get; set; } = 0; // TODO: how to choose the starter?
    public List<Player> Players { get; set; } = new List<Player>();
    
    
}