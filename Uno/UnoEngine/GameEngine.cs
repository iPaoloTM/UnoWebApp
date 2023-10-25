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

    public void SaveGameState()
    {
        Console.Write("Saving Game State...");

        this.GameState.GameDeck = this.CardDeck;

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
           

            string jsonGameState = JsonSerializer.Serialize(this.GameState);


            File.WriteAllText(filePath, jsonGameState);

            Console.WriteLine("Game State exported to JSON successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error exporting game state to JSON: " + ex.Message);
        }
    }



    
}