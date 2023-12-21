using System.ComponentModel.Design.Serialization;

namespace Entities;

public class GameState
{

    public Guid Id { get; set; } = Guid.NewGuid();
    public CardDeck GameDeck { get; set; } = new CardDeck();
    public CardDeck UsedDeck { get; set; } = new CardDeck(new List<Card>());
    public int ActivePlayerNo { get; set; } = 0;
    
    public List<Player> Players { get; set; } = new List<Player>();

    public bool TurnOver { get; set; } = false;
    public bool CanDraw { get; set; } = true;
    public bool EndTurn { get; set; } = false;
    public PlayerMove? LastMove { get; set; }
    
    public int CurrentRoundNo { get; set; } = 0;

    public EColors ColorInPlay { get; set; }

    public bool GameOver { get; set; } = false;

    public static int NumberOfCards { get; set; }  = 112;

    public GameOptions Settings { get; set; } = new GameOptions();
}