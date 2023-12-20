using DAL;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;

namespace RazorWebApp.Pages.Game;

public class Game : PageModel
{
    private readonly GameRepositoryEF _gameRepository = default!;
    public GameEngine Engine;
    public string shoutingText = default!;
    
    [BindProperty]
    public bool IsPlayerTurn { get; set; } = false;

    public Game(GameRepositoryEF gameRepository)
    {
        _gameRepository = gameRepository;
        Engine = new GameEngine(_gameRepository);
    }

    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }

    [BindProperty(SupportsGet = true)] public int PlayerId { get; set; }

    public void OnGet()
    {
        var gameState = _gameRepository.LoadGame(GameId);
        Engine.State = gameState;
        if (PlayerId == Engine.State.ActivePlayerNo) IsPlayerTurn = true;
    }
    
    public void OnPostDrawCard()
    {
        if (Engine.TurnOver)
        {
            //you already acted this turn
        }
        else if (!Engine.CanDraw)
        {
            //you can play one of your cards
        }
        else
        {
            //Console.WriteLine(Engine.State.Id);
            Console.WriteLine("EngineID: "+Engine.State.Id);
        
            var moveDraw = new PlayerMove(Engine.State.Players[Engine.State.ActivePlayerNo], EPlayerAction.Draw, null);
            int success = Engine.HandlePlayerAction(moveDraw);
            if (success != 3 && success != 1)
            {
                //you can play one of your cards
            }
        }
    }

    public void OnPostSkipTurn()
    {
        var gameState = _gameRepository.LoadGame(GameId);
   
        Engine = new GameEngine(_gameRepository)
        {
            State = gameState
        };

        Console.WriteLine(Engine.State.Id);
        var moveSkip = new PlayerMove(Engine.State.Players[Engine.State.ActivePlayerNo], EPlayerAction.NextPlayer, null);
        Engine.HandlePlayerAction(moveSkip);
        if (!Engine.TurnOver)
        {
            //can't end turn without doing smth
        }
        _gameRepository.Save(GameId, Engine.State);
    }

    public void OnPostShout()
    {
        Console.WriteLine(this.shoutingText);
    }
}