// See https://aka.ms/new-console-template for more information

using System.Buffers;
using System.Net.NetworkInformation;

namespace Entities;

public class Player
{
    public string Nickname { get; set; }
    public int Position { get; set; } = default!;
    public EPlayerType PlayerType { get; set; }
    public List<Card> HandCards { get; set; } = new List<Card>();

    public String? Reaction { get; set; }

    public bool SaidUno { get; set; } = false;


    public Player(string nickname = "player", EPlayerType playerType = EPlayerType.Human)
    {
        Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname), "Nickname cannot be null.");
        HandCards = new List<Card>();
        this.PlayerType = playerType;
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
        var playerMove = new PlayerMove(this, EPlayerAction.PlayCard, card);
        return playerMove;
    }

    public PlayerMove Draw()
    {
        var playerMove = new PlayerMove(this, EPlayerAction.Draw, null);
        return playerMove;
    }

    public PlayerMove NextPlayer()
    {
        var playerMove = new PlayerMove(this, EPlayerAction.NextPlayer, null);
        return playerMove;
    }

    public string GetPlayerHand()
    {
        string res = "";

        foreach (Card c in this.HandCards)
        {
            res += c.ToString() + ",";
        }

        return res;
    }

    public override string ToString()
    {
        return "{\"Nickname\":\"" + this.Nickname + "\", \"Position\": " + this.Position + ", \"Hand\":[" +
               this.GetPlayerHand() + "], \"Reaction\": \"" + this.Reaction + "\"}";
    }

    public PlayerMove SaySomething()
    {
        var playerMove = new PlayerMove(this, EPlayerAction.SaySomething, null);
        return playerMove;
    }
}