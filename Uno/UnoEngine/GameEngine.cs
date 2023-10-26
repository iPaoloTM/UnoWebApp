// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Entities;

namespace UnoEngine;

public  sealed class UnoEngine //i removed <TKEY> 
{
    
    public GameState State { get; set; } = new GameState();
    
    public List<Player> Players { get; set; } = new List<Player>();
    public CardHand DrawDeckOfCards { get; set; } = new CardDeck();
    public List<Card> DiscardDeck { get; set; } = new List<Card>();

    public PlayerMove LastTurn { get; set; } = new PlayerMove();
    
    public EColors DominantColor { get; set; } 

    public Validator Validator = new Validator();
    
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
                Players[i].Hand.Add(DrawDeckOfCards.Cards.First());
                DrawDeckOfCards.Cards.RemoveAt(0);
                dealtCards++;
            }
        }
        DiscardDeck.Add(DrawDeckOfCards.Cards.First());
        DominantColor = DiscardDeck.First().Color;
        DrawDeckOfCards.Cards.RemoveAt(0);
  
        while(DiscardDeck.First() is SpecialCard specialCard && (specialCard.Effect == EEffect.Wild || specialCard.Effect == EEffect.DrawFour))
        {
            DiscardDeck.Insert(0, DrawDeckOfCards.Cards.First());
            DominantColor = DiscardDeck.First().Color;
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

    public void HandlePlayerAction(Player player, EPlayerAction playerAction, Card card)
    {
        //Handling "Playing Card"
        if (playerAction != EPlayerAction.PlayCard)
        {
            throw new Exception("something went wrong we need to come up with handling it");
        }
        var response = Validator.validate(card, DiscardDeck.First());
        if (response)
        {
            DiscardDeck.Insert(0, card);
            if (card is SpecialCard specialCard && (specialCard.Effect == EEffect.Wild || specialCard.Effect == EEffect.DrawFour))
            {
                var newColor = player.SelectDominantColor();
                
            }
            player.MakeChoice();
        }
        else
        {
            //we need to show somehow that it's not allowed
            player.MakeChoice();
        }
    }
    
    public void HandlePlayerAction(Player player, EPlayerAction playerAction, List<Card> hand)
    {
        //Handling "Draw"
        if (playerAction != EPlayerAction.Draw)
        {
            throw new Exception("something went wrong we need to come up with handling it");
        }
        var response = Validator.validate(hand, DiscardDeck.First());
        if (response)
        {
            player.Hand.AddRange(DrawDeckOfCards.Draw(1));
            player.MakeChoice();
        }
        else
        {
            player.MakeChoice();
        }
    }
    public void HandlePlayerAction(Player player, EPlayerAction playerAction)
    {
        //Handling "skip"
        if (playerAction == EPlayerAction.NextPlayer)
        {
            var response = Validator.validate();
            if (response)
            {
                //we need to think what to write here guyss :( 
            }
            else
            {
                player.MakeChoice();
            }
        }
        else if (playerAction == EPlayerAction.Shout)
        {
            //we need to think about what will be here
        }
        else
        {
            throw new Exception("something went wrong we need to come up with handling it");
        }
        
    }
    public void PlayGame()
    {
        
    }
}