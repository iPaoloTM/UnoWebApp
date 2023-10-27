// See https://aka.ms/new-console-template for more information

using System.Drawing;
using System.Text.RegularExpressions;
using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnoEngine;

public class GameEngine //i removed <TKEY> 
{
    
    public GameState State { get; set; } = new GameState();
    
    public PlayerMove? LastTurn { get; set; }

    public bool IsAscendingOrder = true;

    public List<Player> Players { get; set; } = new List<Player>();
    public CardDeck CardDeck { get; set; } = new CardDeck();
    public CardDeck UsedDeck { get; set; } = new CardDeck();

    public Validator Val { get; set; } = new Validator();
    
    public PlayerMove? LastMove { get; set; }

    public int ActivePlayerNo, CurrentRoundNo;
    
    private const int InitialHandSize = 7;

    private GameState GameState;

    public GameEngine()
    {
        
    }

    public GameEngine (int numberOfPlayers) // public void setupcards 
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

        ActivePlayerNo = 0;

        int maxNumOfCards = InitialHandSize * numberOfPlayers;
        int dealtCards = 0;
        while(dealtCards < maxNumOfCards)
        {
            for(int i = 0; i < numberOfPlayers; i ++)
            {
                Players[i].HandCards.Add(CardDeck.Cards.First());
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

    public void AddPlayer(string playerName)
    {
        Players.Add(new Player(nickname:playerName)
        {
            PlayerType = EPlayerType.Human
        });

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
                        ////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        Console.WriteLine("Choose a new color");
                        
                        var newColor = Console.ReadLine();
                        
                        //some parsing and sanity check is needed


                    }
                    player.HandCards.Remove(decision.PlayedCard);
                    LastMove = player.PlayCard(decision.PlayedCard);
                }
                else
                {
                    //we need to show somehow that it's not allowed
                    Console.WriteLine("Move not allowed! Retry");
                    //TODO: we need to add interaction with menusystem, maybe we need to return something, so that the menu system knows that we need to show choice one more time !!!
                }
                break;
            case EPlayerAction.Draw:
                response = Val.ValidateAction(LastMove, decision);
                if (response)
                {
                    player.HandCards.Add(GameState.GameDeck.Cards.First());
                    LastMove = player.Draw();
                    LastMove.PlayedCard = UsedDeck.First();
                    HandleUnoShouting(player);

                }
                else
                {
                    Console.WriteLine("Move not allowed! Retry");
                    //TODO: we need to add interaction with menusystem, maybe we need to return something, so that the menu system knows that we need to show choice one more time !!!

                }

                break;
            case EPlayerAction.NextPlayer:
                response = Val.ValidateAction(LastMove, decision);;
                if (response)
                {
                    //we need to think what to write here guyss :(
                    if (IsAscendingOrder)
                    {
                        ActivePlayerNo ++;
                        if (ActivePlayerNo >= Players.Count) //Reset player counter
                        {
                            ActivePlayerNo = 0;
                        }
                    }
                    else
                    {
                        ActivePlayerNo--;
                        if (ActivePlayerNo < 0)
                        {
                            ActivePlayerNo = Players.Count - 1;
                        }
                    }       
                LastMove = player.NextPlayer();
                LastMove.PlayedCard = UsedDeck.First();
                }
                else
                {
                    Console.WriteLine("Move not allowed! Retry");
                }

                break;
            case EPlayerAction.SaySomething:
                var reaction = Console.ReadLine();
                this.Players[ActivePlayerNo].Reaction = reaction;
                HandleUnoShouting(player, reaction);
                if (Regex.Match(reaction, "^Report (0|[1-9]\\d*)$").Success)
                {
                    int playerNumber = int.Parse(Regex.Match(reaction, "^Report (0|[1-9]\\d*)$").Value);
                    HandleUnoReporting(Players[playerNumber]);
                }
                break;
            default:
                throw new Exception("something went wrong when making a decision :(");
        }


    }

    public void HandleUnoShouting(Player player, string message = "")
    {
        if (message == "uno" && player.HandCards.Count() == 1)
        {
            player.SaidUno = true;
        }
        else if (player.SaidUno && player.HandCards.Count() > 1)
        {
            player.SaidUno = false;
        }
    }

    public void HandleUnoReporting(Player player)
    {
        if (!player.SaidUno)
        {
            player.HandCards.AddRange(GameState.GameDeck.Cards.GetRange(0, 2));
            GameState.GameDeck.Cards.RemoveRange(0, 2);
        }
    }

    public void HandleSpecialCard(EEffect effect)
    {
        //TODO: is it handled in validator????
        switch (effect)
        {
            case EEffect.Reverse:
                IsAscendingOrder = !IsAscendingOrder;
                break;
            case EEffect.Skip:
                break;
            case EEffect.Wild:
                break;
            case EEffect.DrawFour:
                break;
            case EEffect.DrawTwo:
                break;
            default:
                throw new Exception("something went wrong");
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
