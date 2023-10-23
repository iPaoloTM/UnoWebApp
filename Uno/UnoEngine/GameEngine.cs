// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Entities;

namespace UnoEngine;

public class UnoEngine<TKey>
{
    
    public GameState State { get; set; } = new GameState();
    private const int InitialHandSize = 7;

        //TODO - to be changed by Kat
        // deal the player hands - - stay in game engine
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