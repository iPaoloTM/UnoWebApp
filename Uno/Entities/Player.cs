// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using Entities;

public class Player
{
    public string Nickname { get; set; } = default!;
    public EPlayerType PlayerTipe { get; set; }

    public List<Card> Deck { get; set; } = new List<Card>();

    public Player()
    {
        Deck = new List<Card>();
    }

    public PlayerTurn PlayTurn()
    {
        //TODO - Kata
    }

    public PlayerTurn DrawCard()
    {
        //TODO - Michal
    }
    
    public void DisplayTurn()
    {
        //TODO - Kata
    }
    
    public PlayerTurn ProccesAttack()
    {
        //TODO - Kata
    }

    private bool hasMatch()
    {
        //TODO - Michal

    }

    private PlayerTurn PlayMatchingCard()
    {
        //TODO - Michal
    }
    
    
}