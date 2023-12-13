using Entities.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;

namespace RazorWebApp.Pages.NewGame;

public class NewGame : PageModel
{
    private readonly GameEngine _gameEngine; // Assuming GameEngine is injected

    public NewGame(GameEngine gameEngine)
    {
        _gameEngine = gameEngine;
    }
    
    [BindProperty]
    public int PlyrNumber { get; set; } = default!;
    
    public void OnGet()
    {
        
    }

    public IActionResult OnPostStart()
    {
        if (!ModelState.IsValid || PlyrNumber < 2 || PlyrNumber > 10)
        {
            return Page();
        }

        // Process the player information
        foreach (var player in Players)
        {
            if (player.IsAI)
            {
                // Add AI player
                _gameEngine.AddPlayer(player.Name);
            }
            else
            {
                // Add human player
                _gameEngine.AddPlayer(player.Name);
            }
        }
    }

}