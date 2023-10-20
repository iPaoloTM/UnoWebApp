// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Entities;

namespace UnoEngine;

public class UnoEngine<TKey>
{
    
    public GameState State { get; set; } = new GameState();
    private const int InitialHandSize = 7;

    //TODO - to be changed
    private void InitializeFullDeck()
    {
        //Initialize Numeric Cards 
        EColors color;
        for (int c = 0; c < 4; c++)
        {
            switch (c)
            {
                case 0:
                    color = EColors.Red;
                    break;
                case 1:
                    color = EColors.Blue;
                    break;
                case 2:
                    color = EColors.Green;
                    break;
                case 3:
                    color = EColors.Yellow;
                    break;
                default:
                    color = EColors.Red;
                    break;
            }

            State.GameDeck.Add(new NumericCard()
            {
                Number = (ENumbers)0,
                Color = color,
            });

            for (int j = 0; j < 2; j++)
            {
                for (int i = 1; i < 13; i++)
                {
                    if (i < 10)
                    {
                        State.GameDeck.Add(new NumericCard()
                        {
                            Number = (ENumbers)i,
                            Color = color,
                        });

                    }
                    else
                    {
                        //Initialize Special Cards
                        EEffect effect;
                        switch (i)
                        {
                            case 10:
                                effect = EEffect.Skip;
                                break;
                            case 11:
                                effect = EEffect.Reverse;
                                break;
                            case 12:
                                effect = EEffect.DrawTwo;
                                break;
                            default:
                                effect = EEffect.DrawTwo;
                                break;
                        }

                        State.GameDeck.Add(new SpecialCard()
                        {
                            Effect = effect,
                            Color = color,
                        });

                    }

                }

                for (int i = 0; i < 2; i++)
                {
                    State.GameDeck.Add(new SpecialCard()
                    {
                        Effect = EEffect.DrawFour,
                        Color = EColors.Black,
                    });
                    State.GameDeck.Add(new SpecialCard()
                    {
                        Effect = EEffect.Wild,
                        Color = EColors.Black,
                    });
                }
            }

        }

        //shuffle  



        // deal the player hands
        for (int playerNo = 0; playerNo < State.Players.Count; playerNo++)
        {
            for (int i = 0; i < InitialHandSize; i++)
            {
                State.Players[playerNo].Deck.Add(State.GameDeck.Last());
                State.GameDeck.RemoveAt(State.GameDeck.Count - 1);
            }
        }

    }
}