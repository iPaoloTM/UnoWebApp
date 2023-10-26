// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Entities;

namespace UnoEngine;

public  sealed class UnoEngine //i removed <TKEY> 
{
    
    public GameState State { get; set; } = new GameState();
    
    
    public List<Player> Players { get; set; } = new List<Player>();
    public CardDeck DrawDeckOfCards { get; set; } = new CardDeck();
    public List<Card> DiscardDeck { get; set; } = new List<Card>();
    
    private const int InitialHandSize = 7;

    private UnoEngine (int numberOfPlayers)
    {
        DrawDeckOfCards.Shuffle();
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Players.Add(new Player()
            {
                Position = i
            });
        }

        int maxNumOfCards = InitialHandSize * numberOfPlayers;
        int dealtCards = 0;
        while(dealtCards < maxNumOfCards)
        {
            for(int i = 0; i < numberOfPlayers; i ++)
            {
                Players[i].Deck.Add(DrawDeckOfCards.Cards.First());
                DrawDeckOfCards.Cards.RemoveAt(0);
                dealtCards++;
            }
        }
        DiscardDeck.Add(DrawDeckOfCards.Cards.First());
        DrawDeckOfCards.Cards.RemoveAt(0);
  
        while(DiscardDeck.First() is SpecialCard specialCard && (specialCard.Effect == EEffect.Wild || specialCard.Effect == EEffect.DrawFour))
        {
            DiscardDeck.Insert(0, DrawDeckOfCards.Cards.First());
            DrawDeckOfCards.Cards.RemoveAt(0);
        }
        
    }
    
    private static UnoEngine? _instance;
    
    
    public static UnoEngine GetInstance(int numberOfPlayers = 2)
    {
        if (_instance == null)
        {
            _instance = new UnoEngine(numberOfPlayers);
        }
        return _instance;
    }
    
    public void PlayGame()
    {
        
    }
}