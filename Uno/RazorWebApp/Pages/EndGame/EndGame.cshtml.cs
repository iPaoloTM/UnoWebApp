using DAL;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;

namespace RazorWebApp.Pages.EndGame;

public class EndGame : PageModel
{
    private readonly GameRepositoryEF _gameRepository = default!;
    public GameEngine Engine;

    [BindProperty (SupportsGet = true)] public Guid GameId { get; set; } = default!;
    [BindProperty (SupportsGet = true)] public string WinPlayer { get; set; } = default!;

    public EndGame(GameRepositoryEF gameRepository)
    {
        _gameRepository = gameRepository;
        Engine = new GameEngine(_gameRepository);
    }

    public void OnGet()
    {
        var gameState = _gameRepository.LoadGame(GameId);
        Engine.State = gameState;
    }
}