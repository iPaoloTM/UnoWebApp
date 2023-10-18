using System.Text.Json;
using DAL;
using Domain;

namespace DurakEngine;

public class GameEngine<TKey, TKey>
{
    public IGameRepository<TKey, TKey> GameRepository { get; set; }

    public GameState State { get; set; } = new GameState();
    
    
    private const int InitialHandSize = 6;
    private Random rnd { get; set; } = new Random();

    public GameEngine(IGameRepository<TKey, TKey> repository)
    {
        InitializeFullDeck();
        GameRepository = repository;
    }



    public void SaveGame()
    {
        GameRepository.SaveGame(null, State);
    }

    void MakeARandomMove()
    {
        var randomCard =
            State.Players[State.ActivePlayerNo]
                .PlayerHand[
                    rnd.Next(State.Players[State.ActivePlayerNo].PlayerHand.Count)
                ];
        InitializePlayers();
        State.CardsInPlayOnTheTable.ReceivedCards.Add(randomCard);
        State.Players[State.ActivePlayerNo].PlayerHand.Remove(randomCard);
    }

    private void InitializePlayers()
    {
        State.Players = new List<Player>()
        {
            new Player()
            {
                NickName = "Puny human",
                PlayerType = EPlayerType.Human
            },
            new Player()
            {
                NickName = "Mighty AI",
                PlayerType = EPlayerType.AI
            },
        };
    }

    void ResponseToMove()
    {
    }
    
    

    private void InitializeFullDeck()
    {
        for (int cardSuite = 0; cardSuite < (int) ECardSuite.Spades; cardSuite++)
        {
            for (int cardValue = 0; cardValue < (int) ECardValue.ValueAce; cardValue++)
            {
                State.DeckOfCardsInPlay.Add(new GameCard()
                {
                    CardSuite = (ECardSuite) cardSuite,
                    CardValue = (ECardValue) cardValue,
                });
            }
        }

        var randomDeck = new List<GameCard>();

        while (State.DeckOfCardsInPlay.Count > 0)
        {
            var randomPositionInDeck = rnd.Next(State.DeckOfCardsInPlay.Count);
            randomDeck.Add(State.DeckOfCardsInPlay[randomPositionInDeck]);
            State.DeckOfCardsInPlay.RemoveAt(randomPositionInDeck);
        }

        State.DeckOfCardsInPlay = randomDeck;


        // deal the player hands
        for (int playerNo = 0; playerNo < State.Players.Count; playerNo++)
        {
            for (int i = 0; i < InitialHandSize; i++)
            {
                State.Players[playerNo].PlayerHand.Add(State.DeckOfCardsInPlay.Last());
                State.DeckOfCardsInPlay.RemoveAt(State.DeckOfCardsInPlay.Count - 1);
            }
        }

        State.TrumpCard = State.DeckOfCardsInPlay.First();
    }
}
