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

    private int _initialHandSize = 7; //Default value


    
    public GameRepositoryEF? GameRepository { get; set; }

    public GameEngine(GameRepositoryEF? gameRepository)
    {
        this.GameRepository = gameRepository;
    }

    public void SetupCards()
    {
        State.GameDeck.Shuffle();

        int maxNumOfCards = _initialHandSize * State.Players.Count;
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

    public int AIplay()
    {
        var maxCards = State.Players[State.ActivePlayerNo].HandCards.Count;

        //PlayerMove[] possibleMoves = new PlayerMove[maxCards];
        List<PlayerMove> list = new List<PlayerMove>();
        
        for (int i = 0; i < maxCards; i++)
        {
            //possibleMoves[i] = new PlayerMove(State.Players[State.ActivePlayerNo], EPlayerAction.PlayCard, State.Players[State.ActivePlayerNo].HandCards[i]);
            list.Add(new PlayerMove(State.Players[State.ActivePlayerNo], EPlayerAction.PlayCard, State.Players[State.ActivePlayerNo].HandCards[i]));
        }
        bool flag = true;
        Random rnd = new Random();
        while (flag)
        {
            int code;
            int rand = rnd.Next(0, list.Count);
            if (list.Count == 0)
            {
                code = HandlePlayerAction(new PlayerMove(State.Players[State.ActivePlayerNo], EPlayerAction.Draw, null));
            }
            else
            {
                
                //Console.WriteLine(possibleMoves[rand]);
                //Console.ReadLine();
                code = HandlePlayerAction(list[rand]);    
            }
            
            switch (code) {
                
                case 0:
                    //Coudln't play card
                    
                    flag = true;
                    //PlayerMove[] possibleMovesTemp = new PlayerMove[list.Count-1]; ;
                    
                    list.RemoveAt(rand);
                    break;
                case 3:
                    //Drew card we can play
                    
                    flag = true;
                    
                    //PlayerMove[] possibleMovesTemp = new PlayerMove[list.Count-1]; ;
                    List<PlayerMove> list2 = new List<PlayerMove>();
                    
                    for (int i = 0; i < State.Players[State.ActivePlayerNo].HandCards.Count; i++)
                    {
                        //possibleMoves[i] = new PlayerMove(State.Players[State.ActivePlayerNo], EPlayerAction.PlayCard, State.Players[State.ActivePlayerNo].HandCards[i]);
                        list2.Add(new PlayerMove(State.Players[State.ActivePlayerNo], EPlayerAction.PlayCard, State.Players[State.ActivePlayerNo].HandCards[i]));
                    }

                    list = list2;
                    break; 
                
                //I guess that we return either if move is ok, or is not. Can we handle the rest elsewhere? 
                case 2:
                    //Played color choosing card
                    Random rnd2 = new Random();
                    var chosenColor = rnd2.Next(1, 4);
                    SetColorInPlay(chosenColor);
                    HandlePlayerAction(new PlayerMove(State.Players[State.ActivePlayerNo], EPlayerAction.NextPlayer, null));
                    flag = false;
                    return code;
                    break;
                case 1:
                    //Drew card
                    HandlePlayerAction(new PlayerMove(State.Players[State.ActivePlayerNo], EPlayerAction.NextPlayer, null));
                    return code;
                default:
                    //Played any other card.
                    HandlePlayerAction(new PlayerMove(State.Players[State.ActivePlayerNo], EPlayerAction.NextPlayer, null));
                    flag = false;
                    return code;
            }
        }

        return -1;
    }


    public void AddPlayer(string playerName, EPlayerType type = EPlayerType.Human)
    {   
        
        State.Players.Add(new Player(nickname: playerName)
        {
            PlayerType = type
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
                        State.TurnOver = true;
                        State.EndTurn = true;
                        return 4;
                    }

                    if (decision.PlayedCard is SpecialCard newSpecialCard)
                    {
                        switch (newSpecialCard.Effect)
                        {
                            case EEffect.Reverse:
                                IsAscendingOrder = !IsAscendingOrder;
                                State.TurnOver = true;
                                break;
                            case EEffect.Skip:
                                State.ActivePlayerNo = NextTurn();
                                State.TurnOver = true;
                                break;
                            case EEffect.Wild:
                                State.TurnOver = true;
                                return 2;
                                break;
                            case EEffect.DrawFour:
                                DrawCards(4, NextTurn());
                                State.ActivePlayerNo = NextTurn();
                                State.TurnOver = true;
                                return 2;
                                break;
                            case EEffect.DrawTwo:
                                DrawCards(2, NextTurn());
                                State.ActivePlayerNo = NextTurn();
                                State.TurnOver = true;
                                break;
                            default:
                                throw new Exception("something went wrong");
                        }
                    }

                    State.ColorInPlay = decision.PlayedCard.Color;

                    State.TurnOver = true;
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
                        State.CanDraw = false;
                        return 3;
                    }

                    State.LastMove = playingPlayer.Draw();
                    State.LastMove.PlayedCard = State.UsedDeck.First();


                    HandleUnoShouting(playingPlayer);
                    State.TurnOver = true;
                    State.CanDraw = false;
                    return 1;
                }
                else
                {
                    return 0;
                }

                break;
            case EPlayerAction.NextPlayer:
                
                
                    if (State.TurnOver)
                    {
                        State.EndTurn = true;
                        State.ActivePlayerNo = NextTurn();
                        State.LastMove = playingPlayer.NextPlayer();
                        State.LastMove.PlayedCard = State.UsedDeck.First();
                        this.GameRepository?.Save(State.Id, State);
                        //NewJSONExport("../SaveGames/game.json"); we may need it later :)
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

    public void NewTurn()
    {
        State.TurnOver = false;
        State.CanDraw = true;
        State.EndTurn = false;
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
            default:
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

    public void SetOptions(GameOptions options)
    {
        this.State.Settings = options;
        this._initialHandSize = options.HandSize;
        this.IsAscendingOrder = options.InitialOrder;
    }
}