// See https://aka.ms/new-console-template for more information

using System.Drawing;
using System.Text.RegularExpressions;
using Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using DAL;

namespace UnoEngine;

public class GameEngine //i removed <TKEY> 
{
    //All attributes (Decks, players, active player...) are held inside the GAME STATE
    public GameState State { get; set; } = new GameState();

    public bool IsAscendingOrder = true;

    public NewValidator Val { get; set; } = new NewValidator();

    private const int InitialHandSize = 7;

    public bool turnOver { get; set; } = false;
    public bool canDraw { get; set; } = true;
    public bool endTurn { get; set; } = false;
    
    public GameRepositoryEF? GameRepository { get; set; }

    public GameEngine(GameRepositoryEF? GameRepository)
    {
        this.GameRepository = GameRepository;
    }

    public void SetupCards()
    {
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
    // 4. Game over, someone played their last card
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
                    if (playingPlayer.HandCards.Count == 0)
                    {
                        State.GameOver = true;
                        turnOver = true;
                        endTurn = true;
                        return 4;
                    }

                    if (decision.PlayedCard is SpecialCard newSpecialCard)
                    {
                        switch (newSpecialCard.Effect)
                        {
                            case EEffect.Reverse:
                                IsAscendingOrder = !IsAscendingOrder;
                                turnOver = true;
                                break;
                            case EEffect.Skip:
                                State.ActivePlayerNo = NextTurn();
                                turnOver = true;
                                break;
                            case EEffect.Wild:
                                turnOver = true;
                                return 2;
                                break;
                            case EEffect.DrawFour:
                                DrawCards(4, NextTurn());
                                State.ActivePlayerNo = NextTurn();
                                turnOver = true;
                                return 2;
                                break;
                            case EEffect.DrawTwo:
                                DrawCards(2, NextTurn());
                                State.ActivePlayerNo = NextTurn();
                                turnOver = true;
                                break;
                            default:
                                throw new Exception("something went wrong");
                        }
                    }

                    State.ColorInPlay = decision.PlayedCard.Color;

                    turnOver = true;
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
                    var drawnCard = State.GameDeck.Cards.First();
                    DrawCards(1, State.ActivePlayerNo);
                    if (Val.CanPlayCard(drawnCard, State))
                    {
                        canDraw = false;
                        return 3;
                    }

                    State.LastMove = playingPlayer.Draw();
                    State.LastMove.PlayedCard = State.UsedDeck.First();


                    HandleUnoShouting(playingPlayer);
                    turnOver = true;
                    canDraw = false;
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
                    if (turnOver)
                    {
                        endTurn = true;
                        State.ActivePlayerNo = NextTurn();
                        State.LastMove = playingPlayer.NextPlayer();
                        State.LastMove.PlayedCard = State.UsedDeck.First();
                        this.GameRepository?.Save(State.Id, State);
                        //NewJSONExport("../SaveGames/game.json"); we may need it later :)
                        return 1;
                    }
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

    public void NewTurn()
    {
        turnOver = false;
        canDraw = true;
        endTurn = false;
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
        var match = Regex.Match(reaction, "^report (0|[1-9]\\d*)$");
        if (match.Success)
        {
            int playerNumber = int.Parse(match.Groups[1].Value);
            if (playerNumber <= State.Players.Count && playerNumber > 0)
            {
                if (!State.Players[playerNumber - 1].SaidUno && State.Players[playerNumber - 1].HandCards.Count == 1)
                {
                    DrawCards(2, playerNumber - 1);
                }
            }
        }
    }

    public void DrawCards(int n, int playerNumber)
    {
        //Check if there are enough cards in the deck 
        if (n > State.GameDeck.Cards.Count)
        {
            //Remove all cards from used deck except first
            var removeUsedDeck = State.UsedDeck.Cards.GetRange(1, State.UsedDeck.Cards.Count - 1);
            //Add all cards to game deck
            State.GameDeck.Cards.AddRange(removeUsedDeck);
            State.GameDeck.Shuffle();
        }

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


    public void NewJSONExport(string filePath)
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        options.Converters.Add(new JsonConverterUno());

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        string json = JsonSerializer.Serialize(this.State, options);

        File.WriteAllText(filePath, json);
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