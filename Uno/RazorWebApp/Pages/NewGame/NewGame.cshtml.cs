using System.Security.Claims;
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

    [BindProperty] public int NumberOfCards { get; set; } = 7;
    
    [BindProperty] public bool InvertedOrder { get; set; }

    
    public void OnGet()
    {

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
        
        foreach (var player in _gameEngine.State.Players)
        {
            Console.WriteLine(player.Nickname);
        }
        
        // Initialize game based on rule type and checkbox states
        if (RuleType == "Classical")
        {
            // Classical rule logic
            var defaultRules = new GameOptions();
            _gameEngine.SetOptions(defaultRules);
        }
        else
        {
            var customRules = new GameOptions();
            Console.WriteLine(InvertedOrder);
            Console.WriteLine(NumberOfCards);
            customRules.InitialOrder = InvertedOrder;
            customRules.HandSize = NumberOfCards;
            _gameEngine.SetOptions(customRules);
        }
  
        
        _gameEngine.SetupCards();

      var gameId = _gameEngine.State.Id; 
      ViewData["GameId"] = gameId;
      _gameRepository.Save(gameId,_gameEngine.State);
      return RedirectToPage("../Game/ChoosePlayer", new { GameId = gameId });
    }

    
}