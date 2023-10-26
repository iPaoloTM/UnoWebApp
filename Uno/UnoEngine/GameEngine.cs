﻿// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnoEngine;

public class UnoEngine //i removed <TKEY> 
{
    
    public GameState State { get; set; } = new GameState();

    public List<Player> Players { get; set; } = new List<Player>();
    public CardDeck CardDeck { get; set; } = new CardDeck();
    public CardDeck UsedDeck { get; set; } = new CardDeck();

    public int ActivePlayerNo, CurrentRoundNo;
    
    private const int InitialHandSize = 7;

    private GameState GameState;

    public UnoEngine (int numberOfPlayers)
    {
        this.GameState = new GameState();
        
        CardDeck.Shuffle();
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
                Players[i].Hand.Add(CardDeck.Cards.First());
                CardDeck.Cards.RemoveAt(0);
                dealtCards++;
            }
        }
        UsedDeck.Add(CardDeck.Cards.First());
        UsedDeck.Cards.RemoveAt(0);
  
        while(UsedDeck.First() is SpecialCard specialCard && (specialCard.Effect == EEffect.Wild || specialCard.Effect == EEffect.DrawFour))
        {
            UsedDeck.Insert(0, CardDeck.Cards.First());
            UsedDeck.Cards.RemoveAt(0);
        }
        
    }


    public void PlayGame()
    {
        
    }
    
    public void HandlePlayerAction(Player player, EPlayerAction playerAction, Card card)
    {
        //Handling "Playing Card"
        if (playerAction != EPlayerAction.PlayCard)
        {
            throw new Exception("something went wrong we need to come up with handling it");
        }
        var response = Validator.ValidateAction(card, UsedDeck.First());
        if (response)
        {
            UsedDeck.Insert(0, card);
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
        var response = Validator.ValidateAction(hand, UsedDeck.First());
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
            var response = Validator.ValidateAction();
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
    
    public void SaveGameState()
    {
        Console.Write("Saving Game State...");

        GameState.GameDeck = this.CardDeck;

        this.GameState.UsedDeck = this.UsedDeck;

        this.GameState.Players = this.Players;

        this.GameState.ActivePlayerNo = this.ActivePlayerNo;
        
        this.GameState.CurrentRoundNo = this.CurrentRoundNo;
        
        
        Console.Write("Game State saved!");
    }

    public void ExportJSON(string filePath)
    {
        Console.Write("Exporting Game State to JSON...");

        try
        {
            string GameState = "{\"CardDeck\":[";

            //include GameDeck's cards

            foreach (Card c in this.GameState.GameDeck.Cards)
            {
                GameState += c.ToString() + ",";
            }

            GameState += "], \"UsedDeck\":[";
            
            //include UsedDeck's cards

            foreach (Card c in this.GameState.UsedDeck.Cards)
            {
                GameState += c.ToString() + ",";
            }
            
            GameState += "], \"Players\":[";
            
            //include Player hand's cards
            
            foreach (Player p in this.GameState.Players)
            {
                GameState += p.ToString() + ",";
            }

            GameState += "],";
            
            //include utilities info

            GameState += "\"ActivePlayerNo\":"+this.GameState.ActivePlayerNo+"\"CurrentRoundNo\":" + this.GameState.CurrentRoundNo + "";
            
            GameState += "}";
            

            string jsonGameState = JsonSerializer.Serialize(GameState);


            File.WriteAllText(filePath, jsonGameState);

            Console.WriteLine("Game State exported to JSON successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error exporting game state to JSON: " + ex.Message);
        }
    }



    
}