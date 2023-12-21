using DAL;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorWebApp.Pages.Game;

public class ChoosePlayer : PageModel
{
    private readonly GameRepositoryEF _gameRepository;
    public List<Player> PlayerList = default!;
    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }
    

    public ChoosePlayer(GameRepositoryEF gameRepository)
    {
        _gameRepository = gameRepository;
    }
    
    public void OnGet()
    {
        var state = _gameRepository.LoadGame(GameId);
        PlayerList = state.Players;
    }
    
    public IActionResult OnPostPlayerChosen(Guid gameId, int playerId) 
    {
        return RedirectToPage("../Game/Game", new { GameId = gameId, PlayerId = playerId });
    }
}