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

var GameRepository = new GameRepositoryEF(db);

string? StartGame(GameEngine gameEngine)
{
    gameEngine.SetupCards();
    GameMenu gameMenu = new GameMenu(gameEngine);
    gameMenu.Run();
    return null;
}

string? LoadGame()
{
    var newEngine = new GameEngine(GameRepository);
    string jsonContent = File.ReadAllText("../SaveGames/game.json");
    
    var options = new JsonSerializerOptions()
    {
        WriteIndented = true
    };
    options.Converters.Add(new JsonConverterUno());
    
    //TODO: implement to be able to choose which game to load, and retrive the correct id :/
    GameState? deserializeState = GameRepository.LoadGame(); //JsonSerializer.Deserialize<GameState>(jsonContent,options);

    newEngine.State = deserializeState;
    GameMenu gameMenu = new GameMenu(newEngine);
    gameMenu.Run();
    return null;
}

string? RunNewGameMenu()
{
    Console.Clear();
    var playerCount = 0;
    var gameEngine = new GameEngine(GameRepository);

    while (true)
    {
        Console.Write($"How many players? [2]:");
        var playerCountStr = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(playerCountStr)) playerCountStr = "2";
        if (int.TryParse(playerCountStr, out playerCount))
        {
            //&& playerCount <= gameEngine.GetMaxAmountOfPlayers()
            if (playerCount > 1 ) break;
        }
    }


    for (int i = 0; i < playerCount; i++)
    {

        string? playerName = "";
        while (true)
        {
            Console.Write($"Player {i + 1} name (min 1 letter):");
            playerName = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "Human" + (i + 1);
            }

            if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length > 0) break;
            Console.WriteLine("Parse error...");
        }

        gameEngine.State.Players.Add(new Player()
        {
            Nickname = playerName,
            PlayerType = EPlayerType.Human
        });
    }
    
    return StartGame(gameEngine);
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
        //To be implemented
    },
});


var userChoice = mainMenu.Run();