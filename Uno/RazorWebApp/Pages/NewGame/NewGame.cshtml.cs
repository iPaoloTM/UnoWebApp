using DAL;
using Entities.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;
using Entities;

namespace RazorWebApp.Pages.NewGame;

public class NewGame : PageModel
{
   private readonly GameEngine _gameEngine;
   private readonly GameRepositoryEF _gameRepository;
   public NewGame(GameEngine gameEngine, GameRepositoryEF gameRepository)
   { 
       _gameEngine = gameEngine;
       _gameRepository = gameRepository;
   }

    [BindProperty]
    public int PlyrNumber { get; set; } 
    
    [BindProperty]
    public int HPlyrNumber { get; set; } 

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
        
        foreach (var player in Players)
        {
            Console.WriteLine(player.Name);
        }
        
    }
    
    public void OnPostConfirmPlayerNumber()
    {
        TempData["PlyrNumber"] = PlyrNumber;
        IsPlayerNumberConfirmed = true;
        Console.WriteLine("Players Number : "+ PlyrNumber);
    }

    public void OnPostConfirmHumanPlayerNumber()
    {
        PlyrNumber = (int)TempData["PlyrNumber"];
        Console.WriteLine("Players Number : "+ PlyrNumber);
        Console.WriteLine("Human Players " + HPlyrNumber);
        Players = new List<PlayerInfo>();
        for (int i = 0; i < HPlyrNumber; i++)
        {
            // Default name for AI players can be set here if needed
            Players.Add(new PlayerInfo{IsAI = false}); // First player is not AI by default
            Console.WriteLine("AAAAAAAAAAAAAAAA");
        }

        for (int j = 0; j < PlyrNumber-HPlyrNumber; j++)
        {
            Players.Add(new PlayerInfo{IsAI = true});
            Console.WriteLine("OOOOOOOOOOOOOOOOOO");
        }
        
        IsHumanPlayerNumberConfirmed = true;
    }

    public IActionResult OnPostStart()
    {
        if (!ModelState.IsValid || PlyrNumber < 2 || PlyrNumber > 10)
        {
            return Page();
        }

        // Process the player information
        for (int i = 0; i< Players.Count; i++)
        {
            if (Players[i].IsAI)
            {
                // Add AI player
                _gameEngine.AddPlayer("PlayerGPT " + (i + 1), EPlayerType.AI);
                
            }
            else
            {
                // Add human player
                var name = string.IsNullOrWhiteSpace(Players[i].Name) ? $"Player {i+1}" : Players[i].Name;
                _gameEngine.AddPlayer(name);
            }
        }
        Console.WriteLine("AOAOAOAOOAOAOAOAOAOAO");
        foreach (var player in _gameEngine.State.Players)
        {
            Console.WriteLine(player.Nickname);
        }
        
        // Initialize game based on rule type and checkbox states
        if (RuleType == "Classical")
        {
            // Classical rule logic
            var defaultRules = new GameOptions();
            _gameEngine.State.Settings = defaultRules;
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
      _gameRepository.Save(gameId,_gameEngine.State);
      return RedirectToPage("../Game/ChoosePlayer", new { GameId = gameId });
    }
    public class PlayerInfo
    {
        public string? Name { get; set; }
        public bool IsAI { get; set; }
    }
    
}