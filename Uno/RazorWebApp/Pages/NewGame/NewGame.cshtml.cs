using DAL;
using Entities.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;
using Entities;
using Player = Entities.Player;

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
    public List<Player> Players { get; set; } = new List<Player>();

    [BindProperty]
    public bool IsPlayerNumberConfirmed { get; set; } = false;
    
    [BindProperty]
    public bool IsHumanPlayerNumberConfirmed { get; set; } = false;

    [BindProperty]
    public List<string> nicknameList { get; set; } = new List<string>();

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
        Color1 = "#ff0000"; // Example default color
        Color2 = "#00ff00";
        Color3 = "#0000ff";
        Color4 = "#ffff00";
        for (int i = 0; i < 10; i++)
        {
            nicknameList.Add("");
        }
    }
    
    public void OnPostConfirmPlayerNumber(int generalPlayerNumber)
    {
        Console.WriteLine(generalPlayerNumber);
        PlyrNumber = generalPlayerNumber;
        TempData["PlyrNumber"] = generalPlayerNumber;
        IsPlayerNumberConfirmed = true;
        Console.WriteLine("Players Number : "+ generalPlayerNumber);
    }

    public void OnPostConfirmHumanPlayerNumber(int humanPlayerNumber)
    {
        HPlyrNumber = humanPlayerNumber;
        TempData["HPlyrNumber"] = humanPlayerNumber;
        Console.WriteLine("Human Players " + humanPlayerNumber);
        for (int i = 0; i < humanPlayerNumber; i++)
        {
            nicknameList.Add("");
        }
        IsHumanPlayerNumberConfirmed = true;
        
    }

    public IActionResult OnPostStart()
    {
        int hPlyrNumber = (int)TempData["HPlyrNumber"];
        int totalPlyrNumber = (int)TempData["PlyrNumber"];
        if (!ModelState.IsValid || totalPlyrNumber < 2 || totalPlyrNumber > 10)
        {
            return Page();
        }

        for (int i = 0; i< totalPlyrNumber-hPlyrNumber; i++)
        {
            _gameEngine.AddPlayer("PlayerGPT " + (i+1),EPlayerType.AI);
        }
        Console.WriteLine(nicknameList.Count);
        for (int i = 0; i < hPlyrNumber;i++)
        {
            _gameEngine.AddPlayer(nicknameList[i]);
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

    
}