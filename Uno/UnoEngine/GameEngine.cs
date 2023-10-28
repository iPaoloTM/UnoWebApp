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
        while (dealtCards < maxNumOfCards)
        {
            for (int i = 0; i < State.Players.Count; i++)
            {
                State.Players[i].HandCards.Add(State.GameDeck.Cards.First());
                State.GameDeck.Cards.RemoveAt(0);
                dealtCards++;
            }
        }

        State.UsedDeck.Add(State.GameDeck.Cards.First());
        State.GameDeck.Cards.RemoveAt(0);

        while (State.UsedDeck.First() is SpecialCard specialCard &&
               (specialCard.Effect == EEffect.Wild || specialCard.Effect == EEffect.DrawFour))
        {
            State.UsedDeck.Insert(0, State.GameDeck.Cards.First());
            State.GameDeck.Cards.RemoveAt(0);
        }
    }


    public void AddPlayer(string playerName)
    {
        State.Players.Add(new Player(nickname: playerName)
        {
            PlayerType = EPlayerType.Human
        });
    }

    //this function will be called by MenuSystem
    // Return int types:
    // 0. False
    // 1. True
    // 2. Need to choose color
    // 3. Can play drawn card
    public int HandlePlayerAction(PlayerMove decision)
    {
        var response = false;
        var playingPlayer = State.Players[State.ActivePlayerNo];
        //Handling "Playing Card"
        switch (decision.PlayerAction)
        {
            case EPlayerAction.PlayCard:
                response = Val.ValidateMove(decision, State);
                if (response)
                {
                    State.UsedDeck.Insert(0, decision.PlayedCard);

                    playingPlayer.HandCards.Remove(decision.PlayedCard);
                    State.LastMove = playingPlayer.PlayCard(decision.PlayedCard);
                    if (decision.PlayedCard is SpecialCard newSpecialCard)
                    {
                        switch (newSpecialCard.Effect)
                        {
                            case EEffect.Reverse:
                                IsAscendingOrder = !IsAscendingOrder;
                                break;
                            case EEffect.Skip:
                                State.ActivePlayerNo = NextTurn();
                                break;
                            case EEffect.Wild:
                                Console.WriteLine("WILD");
                                Console.ReadLine();
                                return 2;
                                break;
                            case EEffect.DrawFour:
                                DrawCards(4, NextTurn());
                                State.ActivePlayerNo = NextTurn();
                                Console.WriteLine("d4");
                                Console.ReadLine();
                                return 2;
                                break;
                            case EEffect.DrawTwo:
                                DrawCards(2, NextTurn());
                                State.ActivePlayerNo = NextTurn();
                                break;
                            default:
                                throw new Exception("something went wrong");
                        }
                    }

                    State.ColorInPlay = decision.PlayedCard.Color;

                    return 1;
                }
                else
                {
                    return 0;
                }

                break;
            case EPlayerAction.Draw:
                response = Val.ValidateMove(decision, State);
                if (response)
                {
                    playingPlayer.HandCards.Add(State.GameDeck.Cards.First());
                    if (Val.CanPlayCard(State.GameDeck.Cards.First(), State))
                    {
                        return 3;
                    }
                    State.LastMove = playingPlayer.Draw();
                    State.GameDeck.Cards.RemoveAt(0);
                    State.LastMove.PlayedCard = State.UsedDeck.First();
                    
                    

                    HandleUnoShouting(playingPlayer);
                    return 1;
                }
                else
                {
                    return 0;
                }

                break;
            case EPlayerAction.NextPlayer:
                response = Val.ValidateMove(decision, State);
                if (response)
                {
                    //we need to think what to write here guyss :(
                    State.ActivePlayerNo = NextTurn();
                    State.LastMove = playingPlayer.NextPlayer();
                    State.LastMove.PlayedCard = State.UsedDeck.First();
                    return 1;
                }
                else
                {
                    return 0;
                }

                break;
            case EPlayerAction.SaySomething:
                var reaction = Console.ReadLine();
                State.Players[State.ActivePlayerNo].Reaction = reaction;
                HandleUnoShouting(playingPlayer, reaction);

                break;
            default:
                throw new Exception("something went wrong when making a decision :(");
        }

        return 0;
    }

    public void HandleUnoShouting(Player player, string? message = "")
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

    public void HandleUnoReporting(string reaction)
    {
        if (Regex.Match(reaction, "^Report (0|[1-9]\\d*)$").Success)
        {
            int playerNumber = int.Parse(Regex.Match(reaction, "^Report (0|[1-9]\\d*)$").Value);
            if (playerNumber > State.Players.Count && !State.Players[playerNumber].SaidUno)
            {
                DrawCards(2, playerNumber);
            }
        }
    }

    public void DrawCards(int n, int playerNumber)
    {
        State.Players[playerNumber].HandCards.AddRange(State.GameDeck.Cards.GetRange(0, n));
        State.GameDeck.Cards.RemoveRange(0, n);
    }

    public void SetColorInPlay(int color)
    {
        switch (color)
        {
            case 1:
                State.ColorInPlay = EColors.Red;
                break;
            case 2:
                State.ColorInPlay = EColors.Blue;
                break;
            case 3:
                State.ColorInPlay = EColors.Yellow;
                break;
            case 4:
                State.ColorInPlay = EColors.Green;
                break;
        }
    }

    public int NextTurn()
    {
        if (IsAscendingOrder)
        {
            if (State.ActivePlayerNo + 1 >= State.Players.Count) //Reset player counter
            {
                return 0;
            }
            else return State.ActivePlayerNo + 1;
        }
        else
        {
            if (State.ActivePlayerNo - 1 < 0)
            {
                return State.Players.Count - 1;
            }
            else return State.ActivePlayerNo - 1;
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

            GameState += "\"ActivePlayerNo\":" + this.State.ActivePlayerNo + "\"CurrentRoundNo\":" +
                         this.State.CurrentRoundNo + "";

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