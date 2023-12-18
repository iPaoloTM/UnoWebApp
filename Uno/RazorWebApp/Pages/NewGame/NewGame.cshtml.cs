using Entities.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;

namespace RazorWebApp.Pages.NewGame;

public class NewGame : PageModel
{
   private readonly GameEngine _gameEngine; 
   public NewGame(GameEngine gameEngine)
   { 
       _gameEngine = gameEngine;
   }

    [BindProperty]
    public int PlyrNumber { get; set; } = 5;
    
    [BindProperty]
    public int HPlyrNumber { get; set; } = 5;

    [BindProperty]
    public List<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();

    [BindProperty]
    public bool IsPlayerNumberConfirmed { get; set; } = false;
    
    [BindProperty]
    public bool IsHumanPlayerNumberConfirmed { get; set; } = false;

    

    [BindProperty] public string RuleType { get; set; } = "Classical";

    [BindProperty]
    public bool BlockedCard { get; set; }

    [BindProperty]
    public bool WildCards { get; set; }
    
    [BindProperty]
    public string Color1 { get; set; } = "#ff0000";

    [BindProperty]
    public string Color2 { get; set; } = "#00ff00";

    [BindProperty]
    public string Color3 { get; set; } = "#ff0ff0";

    [BindProperty]
    public string Color4 { get; set; } = "#ff00ff";
    
    public void OnGet()
    {
        // Initialize with default values
        for (int i = 0; i < PlyrNumber; i++)
        {
            Players.Add(new PlayerInfo { IsAI = false });
        }
        
        Color1 = "#ff0000"; // Example default color
        Color2 = "#00ff00";
        Color3 = "#0000ff";
        Color4 = "#ffff00";
    }
    private void InitializePlayers()
    {
        Players = new List<PlayerInfo>();
        for (int i = 0; i < HPlyrNumber; i++)
        {
            // Default name for AI players can be set here if needed
            Players.Add(new PlayerInfo{IsAI = false}); // First player is not AI by default
        }

        for (int j = HPlyrNumber; j < PlyrNumber; j++)
        {
            Players.Add(new PlayerInfo{IsAI = true});
        }
    }
    
    public void OnPostConfirmPlayerNumber()
    {
        IsPlayerNumberConfirmed = true;
    }

    public void OnPostConfirmHumanPlayerNumber()
    {
        InitializePlayers();
        IsHumanPlayerNumberConfirmed = true;
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
        
        // Initialize game based on rule type and checkbox states
        if (RuleType == "Classical")
        {
            // Classical rule logic
            
        }
        else
        {
            var colors = new List<string> { Color1, Color2, Color3, Color4 };
            if (colors.Distinct().Count() != colors.Count)
            {
                ModelState.AddModelError("", "Duplicate colors are not allowed.");
                return Page();
            }
            if (BlockedCard)
            {
                
            }

            if (WildCards)
            {
                
            }
        }
        
        _gameEngine.SetupCards();

      var gameId = _gameEngine.State.Id; 
      ViewData["GameId"] = gameId;
      return RedirectToPage("../Game/Game", new { GameId = gameId, PlayerId = _gameEngine.State.ActivePlayerNo });
    }
    public class PlayerInfo
    {
        public string? Name { get; set; }
        public bool IsAI { get; set; }
    }
    
}