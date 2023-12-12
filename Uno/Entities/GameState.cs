using System.ComponentModel.Design.Serialization;

namespace Entities;

public class GameState
{

    public Guid Id { get; set; } = Guid.NewGuid();
    public CardDeck GameDeck { get; set; } = new CardDeck();
    public CardDeck UsedDeck { get; set; } = new CardDeck(new List<Card>());
    public int ActivePlayerNo { get; set; } = 0;
    public List<Player> Players { get; set; } = new List<Player>();

    public PlayerMove? LastMove { get; set; }
    public int CurrentRoundNo { get; set; } = 0;

    public EColors ColorInPlay;

    public bool GameOver = false;

    public static int NumberOfCards = 112;

    public GameOptions Settings = new GameOptions();
}