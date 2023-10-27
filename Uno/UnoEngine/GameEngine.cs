// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnoEngine;

public class GameEngine //i removed <TKEY> 
{
    
    public GameState GameState { get; set; } =  new GameState();
    
    public PlayerMove? LastTurn { get; set; } 

    public List<Player> Players { get; set; } = new List<Player>();
    public CardDeck CardDeck { get; set; } = new CardDeck();
    public CardDeck UsedDeck { get; set; } = new CardDeck();

    public Validator Val { get; set; } = new Validator();
    
    public PlayerMove? LastMove { get; set; }

    public int ActivePlayerNo, CurrentRoundNo;
    
    private const int InitialHandSize = 7;
    

    public void SetupCards()
    {
        //Constructor on CardDeck automatically creates another deck
        //So I do this to avoid having 200+ cards on the table
        //TODO: Fix 2 decks intialized
        UsedDeck.Cards = new List<Card>();
        CardDeck.Shuffle();

        int maxNumOfCards = InitialHandSize * Players.Count;
        int dealtCards = 0;
        while(dealtCards < maxNumOfCards)
        {
            for(int i = 0; i < Players.Count; i ++)
            {
                Players[i].HandCards.Add(CardDeck.Cards.First());
                CardDeck.Cards.RemoveAt(0);
                dealtCards++;
            }
        }
        UsedDeck.Add(CardDeck.Cards.First());
        CardDeck.Cards.RemoveAt(0);
  
        while(UsedDeck.First() is SpecialCard specialCard && (specialCard.Effect == EEffect.Wild || specialCard.Effect == EEffect.DrawFour))
        {
            UsedDeck.Insert(0, CardDeck.Cards.First());
            CardDeck.Cards.RemoveAt(0);
        }
        
    }
    
    //this function will be called by MenuSystem
    public void HandlePlayerAction(Player player, PlayerMove decision)
    {
        var response = false;
        //Handling "Playing Card"
        switch (decision.PlayerAction)
        {
            case EPlayerAction.PlayCard:
                response = Val.ValidateAction(LastMove,decision);
                if (response)
                {
                    UsedDeck.Insert(0, decision.PlayedCard);
                    if (decision.PlayedCard is SpecialCard specialCard && (specialCard.Effect == EEffect.Wild || specialCard.Effect == EEffect.DrawFour))
                    {
                        
                        Console.WriteLine("Choose a new color");
                        
                        var newColor = Console.ReadLine();
                        
                        //some parsing and sanity check is needed


                    }
                    player.PlayCard(decision.PlayedCard);
                }
                else
                {
                    //we need to show somehow that it's not allowed
                    Console.WriteLine("Move not allowed! Retry");
                }
                break;
            case EPlayerAction.Draw:
                response = Val.ValidateAction(LastMove, decision);
                if (response)
                {
                    player.HandCards.Add(GameState.GameDeck.Cards.First());
                    
                }
                else
                {
                    Console.WriteLine("Move not allowed! Retry");
                }

                break;
            // Skip
            case EPlayerAction.NextPlayer:
                response = Val.ValidateAction(LastMove, decision);;
                if (response)
                {
                    //we need to think what to write here guyss :( 
                }
                else
                {
                    Console.WriteLine("Move not allowed! Retry");
                }

                break;
            case EPlayerAction.SaySomething:
                var reaction = Console.ReadLine();
                //Im missing something here
                //this.Players[ActivePlayerNo].Reaction = reaction;
                break;
            default:
                throw new Exception("something went wrong when making a decision :(");
        }

        LastMove = decision;

    }
    
    
    public void SaveGameState()
    {
        Console.Write("Saving Game State...");

        GameState.GameDeck = this.CardDeck;

        this.GameState.UsedDeck = this.UsedDeck;

        this.GameState.Players = this.Players;

        this.GameState.ActivePlayerNo = this.ActivePlayerNo;
        
        this.GameState.CurrentRoundNo = this.CurrentRoundNo;
        
        this.GameState.LastMove = this.LastMove;
        
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
            
            
            //TODO: serialize this.GameState? Teacher does it in his example and is simpler
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