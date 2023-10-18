// See https://aka.ms/new-console-template for more information

using DAL;
using Domain;
using DurakEngine;
using MenuSystem;

var gameRepository = new GameRepositoryFileSystem();
var game = new GameEngine<string, string>(gameRepository);

string? SetPlayerCount()
{
    Console.Write("Player count?");
    var countStr = Console.ReadLine()?.Trim();
    var count = int.Parse(countStr);

    game.Players = new List<Player>();
    for (int i = 0; i < count; i++)
    {
        game.Players.Add(new Player()
        {
            NickName   = "Human " + i,
            PlayerType = EPlayerType.Human,
        });
    }
    return null;
}



string? runNewGameMenu()
{
    var startNewGameMenu = new Menu("New Game", new List<MenuItem>()
        {
            new MenuItem()
            {
                Shortcut = "c",
                MenuLabel = "Player count: " + game.Players.Count,
                MethodToRun = SetPlayerCount
            },
            new MenuItem()
            {
                Shortcut = "t",
                MenuLabel = "Player names and types: ",
            },
            new MenuItem()
            {
                Shortcut = "s",
                MenuLabel = "Start the game of DURAK",
            },
        }
    );
    
    
    return startNewGameMenu.Run(EMenuLevel.Second);
}

var mainMenu = new Menu(">> D U R A K <<", new List<MenuItem>()
{
    new MenuItem()
    {
        Shortcut = "s",
        MenuLabel = "Start a new game",
        MethodToRun = runNewGameMenu
    },
    new MenuItem()
    {
        Shortcut = "l",
        MenuLabel = "Load game",
    },
    new MenuItem()
    {
        Shortcut = "o",
        MenuLabel = "Options",
    },
});


var userChoice = mainMenu.Run();