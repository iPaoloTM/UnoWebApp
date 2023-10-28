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
    
    //All attributes (Decks, players, active player...) are held inside the GAME STATE
    public GameState State { get; set; } = new GameState();
    
    public bool IsAscendingOrder = true;

    public NewValidator Val { get; set; } = new NewValidator();
    
    private const int InitialHandSize = 7;
    
    public void SetupCards()
    {
        //Constructor on CardDeck automatically creates another deck
        //So I do this to avoid having 200+ cards on the table
        //TODO: Fix 2 decks initialized
        State.UsedDeck.Cards = new List<Card>();
        State.GameDeck.Shuffle();

        int maxNumOfCards = InitialHandSize * State.Players.Count;
        int dealtCards = 0;
        while(dealtCards < maxNumOfCards)
        {
            for(int i = 0; i < State.Players.Count; i ++)
            {
                State.Players[i].HandCards.Add(State.GameDeck.Cards.First());
                State.GameDeck.Cards.RemoveAt(0);
                dealtCards++;
            }
        }
        State.UsedDeck.Add(State.GameDeck.Cards.First());
        State.GameDeck.Cards.RemoveAt(0);
  
        while(State.UsedDeck.First() is SpecialCard specialCard && (specialCard.Effect == EEffect.Wild || specialCard.Effect == EEffect.DrawFour))
        {
            State.UsedDeck.Insert(0, State.GameDeck.Cards.First());
            State.GameDeck.Cards.RemoveAt(0);
        }
        
    }


    public void AddPlayer(string playerName)
    {
        State.Players.Add(new Player(nickname:playerName)
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
                response = Val.ValidateMove(decision,State);
                if (response)
                {
                    State.UsedDeck.Insert(0, decision.PlayedCard);
                    if (decision.PlayedCard is SpecialCard specialCard && (specialCard.Effect == EEffect.Wild || specialCard.Effect == EEffect.DrawFour))
                    {
                        ////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        Console.WriteLine("Choose a new color");
                        
                        var newColor = Console.ReadLine();
                        
                        //some parsing and sanity check is needed


                    }
                    player.HandCards.Remove(decision.PlayedCard);
                    State.LastMove = player.PlayCard(decision.PlayedCard);
                }
                else
                {
                    //we need to show somehow that it's not allowed
                    Console.WriteLine("Move not allowed! Retry");
                    //TODO: we need to add interaction with menusystem, maybe we need to return something, so that the menu system knows that we need to show choice one more time !!!
                }
                break;
            case EPlayerAction.Draw:
                response = Val.ValidateMove(decision,State);
                if (response)
                {
                    player.HandCards.Add(State.GameDeck.Cards.First());
                    State.LastMove = player.Draw();
                    State.LastMove.PlayedCard = State.UsedDeck.First();
                    HandleUnoShouting(player);

                }
                else
                {
                    Console.WriteLine("Move not allowed! Retry");
                    //TODO: we need to add interaction with menusystem, maybe we need to return something, so that the menu system knows that we need to show choice one more time !!!

                }

                break;
            case EPlayerAction.NextPlayer:
                response = Val.ValidateMove(decision,State);
                if (response)
                {
                    //we need to think what to write here guyss :(
                    if (IsAscendingOrder)
                    {
                        State.ActivePlayerNo ++;
                        if (State.ActivePlayerNo >= State.Players.Count) //Reset player counter
                        {
                            State.ActivePlayerNo = 0;
                        }
                    }
                    else
                    {
                        State.ActivePlayerNo--;
                        if (State.ActivePlayerNo < 0)
                        {
                            State.ActivePlayerNo = State.Players.Count - 1;
                        }
                    }       
                State.LastMove = player.NextPlayer();
                State.LastMove.PlayedCard = State.UsedDeck.First();
                }
                else
                {
                    Console.WriteLine("Move not allowed! Retry");
                }

                break;
            case EPlayerAction.SaySomething:
                var reaction = Console.ReadLine();
                State.Players[State.ActivePlayerNo].Reaction = reaction;
                HandleUnoShouting(player, reaction);
                if (Regex.Match(reaction, "^Report (0|[1-9]\\d*)$").Success)
                {
                    int playerNumber = int.Parse(Regex.Match(reaction, "^Report (0|[1-9]\\d*)$").Value);
                    HandleUnoReporting(State.Players[playerNumber]);
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
            player.HandCards.AddRange(State.GameDeck.Cards.GetRange(0, 2));
            State.GameDeck.Cards.RemoveRange(0, 2);
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

    public void ExportJSON(string filePath)
    {
        Console.Write("Exporting Game State to JSON...");

        try
        {
            string GameState = "{\"CardDeck\":[";

            //include GameDeck's cards

            foreach (Card c in this.State.GameDeck.Cards)
            {
                GameState += c.ToString() + ",";
            }

            GameState += "], \"UsedDeck\":[";
            
            //include UsedDeck's cards

            foreach (Card c in this.State.UsedDeck.Cards)
            {
                GameState += c.ToString() + ",";
            }
            
            GameState += "], \"Players\":[";
            
            //include Player hand's cards
            
            foreach (Player p in this.State.Players)
            {
                GameState += p.ToString() + ",";
            }

            GameState += "],";
            
            //include utilities info

            GameState += "\"ActivePlayerNo\":"+this.State.ActivePlayerNo+"\"CurrentRoundNo\":" + this.State.CurrentRoundNo + "";
            
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

