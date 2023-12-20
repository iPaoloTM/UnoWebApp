// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using DAL;
using Entities;
using Entities.Database;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using UnoEngine;
using Player = Entities.Player;

var contextOptions = new DbContextOptionsBuilder<UnoDbContext>()
    .UseSqlite("Data Source=app.db")
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;

using var db = new UnoDbContext(contextOptions);
db.Database.Migrate();

var gameRepository = new GameRepositoryEF(db);
var options = new GameOptions();

string? StartGame(GameEngine gameEngine)
{
    gameEngine.SetupCards();
    GameMenu gameMenu = new GameMenu(gameEngine);
    gameMenu.Run();
    return null;
}

string? LoadGame()
{
    var saveGames = gameRepository.GetSaveGames();
    var saveGameListDisplay = saveGames.Select((s, i) => (i + 1) + " - " + s).ToList();

    if (saveGameListDisplay.Count == 0) return null;

    Guid? gameId = null;
    while (true)
    {
        Console.WriteLine(string.Join("\n", saveGameListDisplay));
        Console.Write($"Select game to load (1..{saveGameListDisplay.Count}):");
        var userChoiceStr = Console.ReadLine();
        if (int.TryParse(userChoiceStr, out var parsedChoice))
        {
            if (parsedChoice < 0 || parsedChoice > saveGameListDisplay.Count)
            {
                Console.WriteLine("Not in range...");
                continue;
            }

            gameId = saveGames[parsedChoice - 1].id;
            Console.WriteLine($"Loading file: {gameId}");

            break;
        }
    }

    var newEngine = new GameEngine(gameRepository);
    //string jsonContent = File.ReadAllText("../SaveGames/game.json");

    var options = new JsonSerializerOptions()
    {
        WriteIndented = true
    };
    options.Converters.Add(new JsonConverterUno());

    GameState?
        deserializeState =
            gameRepository.LoadGame(gameId); 
    newEngine.State = deserializeState;
    GameMenu gameMenu = new GameMenu(newEngine);
    gameMenu.Run();

    return null;
}

string? RunNewGameMenu()
{
    Console.Clear();
    var playerCount = 0;
    var gameEngine = new GameEngine(gameRepository);
    var humanCount = 0;

    while (true)
    {
        playerCount = PromptValidator.UserPrompt("How many human players? [2]:", 1, 10);
        if (playerCount == -1) playerCount = 2;
        break;
    }

    while (true)
    {
        humanCount = PromptValidator.UserPrompt("How many of these players are human? [2]:", 0, playerCount);
        if ((humanCount is int) && (humanCount >= 0) && (humanCount <= playerCount))
        {
            break;
        }

        humanCount = 2;
    }
    
    for (int i = 0; i < humanCount; i++)
    {
        string? playerName = "";
        while (true)
        {

            playerName = PromptValidator.UserPrompt($"Player {i + 1} name (min 1 letter):");
            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "Human" + (i + 1);
            }
            else
            {
                playerName=playerName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length > 0) break;
            Console.WriteLine("Parse error...");
        }

        gameEngine.AddPlayer(playerName);
        
    }

    var AIcount = playerCount - humanCount;

    if (AIcount > 0)
    {
        for (int i = 0; i < AIcount; i++)
        {
            var AIName = "PlayerGPT " + (i + 1);
            gameEngine.AddPlayer(AIName, EPlayerType.AI);
        }
    }
    
    //
    gameEngine.SetOptions(options);

    return StartGame(gameEngine);
}

string? ChangeOptions()
{
    var optMenu = new OptionsMenu();
    options = optMenu.Run();
    return null;
}

var mainMenu = new Menu(">> U N O <<", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "s",
        MenuLabel = "Start a new game",
        MethodToRun = RunNewGameMenu
    },
    new MenuItem()
    {
        Shortcut = "l",
        MenuLabel = "Load game",
        MethodToRun = LoadGame
    },
    new MenuItem()
    {
        Shortcut = "o",
        MenuLabel = "Options",
        MethodToRun = ChangeOptions
    },
    new MenuItem()
    {
        Shortcut = "x",
        MenuLabel = "Exit",
    },
});


var userChoice = mainMenu.Run();