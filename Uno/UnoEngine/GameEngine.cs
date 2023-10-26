// See https://aka.ms/new-console-template for more information

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
    public CardHand CardHand { get; set; } = new CardHand();
    public CardHand UsedHand { get; set; } = new CardHand();

    public int ActivePlayerNo, CurrentRoundNo;
    
    private const int InitialHandSize = 7;

    private GameState GameState;

    public UnoEngine (int numberOfPlayers)
    {
        this.GameState = new GameState();
        
        CardHand.Shuffle();
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
                Players[i].Hand.Add(CardHand.Cards.First());
                CardHand.Cards.RemoveAt(0);
                dealtCards++;
            }
        }
        UsedHand.Add(CardHand.Cards.First());
        UsedHand.Cards.RemoveAt(0);
  
        while(UsedHand.First() is SpecialCard specialCard && (specialCard.Effect == EEffect.Wild || specialCard.Effect == EEffect.DrawFour))
        {
            UsedHand.Insert(0, CardHand.Cards.First());
            UsedHand.Cards.RemoveAt(0);
        }
        
    }

    public void PlayGame()
    {
        
    }

    public void SaveGameState()
    {
        Console.Write("Saving Game State...");

        this.GameState.GameHand = this.CardHand;

        this.GameState.UsedHand = this.UsedHand;

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

            foreach (Card c in this.GameState.GameHand.Cards)
            {
                GameState += c.ToString() + ",";
            }

            GameState += "], \"UsedDeck\":[";
            
            //include UsedDeck's cards

            foreach (Card c in this.GameState.UsedHand.Cards)
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