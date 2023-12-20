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
    [BindProperty(SupportsGet = true)] 
    public Guid GameId { get; set; }

    [BindProperty(SupportsGet = true)] 
    public int PlayerId { get; set; }

    public Game(GameRepositoryEF gameRepository)
    {
        _gameRepository = gameRepository;
        Engine = new GameEngine(_gameRepository);
    }



    public void OnGet()
    {
        var gameState = _gameRepository.LoadGame(GameId);
        Engine.State = gameState;
        if (PlayerId == Engine.State.ActivePlayerNo) IsPlayerTurn = true;
        
    }
    
    public void OnPostDrawCard(Guid gameId,int currPlayer)
    {
        /* TODO
         For some reason i cannot comprehend, if I dont load the state every time from the 
         database the engine accessed is a new instance without the relevant game information
         I have no clue how to fix this
         */
        var currState = _gameRepository.LoadGame(gameId);
        Engine.State = currState;
        
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
        
            var moveDraw = new PlayerMove(Engine.State.Players[Engine.State.ActivePlayerNo], EPlayerAction.Draw, null);
            int success = Engine.HandlePlayerAction(moveDraw);
            if (success != 3 && success != 1)
            {
                //you can play one of your cards
            }
        }
    }

    public IActionResult OnPostSkipTurn(Guid gameId,int currPlayer)
    {
        var currState = _gameRepository.LoadGame(GameId);
        Engine.State = currState;
        var moveSkip = new PlayerMove(Engine.State.Players[currPlayer], EPlayerAction.NextPlayer,
            null);
        int response = Engine.HandlePlayerAction(moveSkip);
        //Conditions to end turn are met
        if (response == 1)
        {
            _gameRepository.Save(gameId,Engine.State);
        }

        return RedirectToPage("../Game/Game", new { GameId = gameId, PlayerId = currPlayer });
    }

    public void OnPostShout()
    {
        Console.WriteLine(this.shoutingText);
    }
}