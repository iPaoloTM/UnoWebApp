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
}