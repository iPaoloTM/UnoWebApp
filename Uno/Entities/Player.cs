// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using Entities;

public class Player
{
    public string Nickname { get; set; } = default!;
    public EPlayerType PlayerTipe { get; set; }

    public List<Card> Deck { get; set; } = new List<Card>();
    
}