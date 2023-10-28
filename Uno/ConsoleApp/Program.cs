// See https://aka.ms/new-console-template for more information

using Entities;
using MenuSystem;
using UnoEngine;


string? startGame(GameEngine gameEngine)
{
    gameEngine.SetupCards();
    GameMenu gameMenu = new GameMenu(gameEngine);
    gameMenu.Run();
    return null;
}


string? runNewGameMenu()
{
    Console.Clear();
    var playerCount = 0;
    var gameEngine = new GameEngine();

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
    
    return startGame(gameEngine);
}

var mainMenu = new Menu(">> U N O <<", new List<MenuItem>()
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
        //To be implemented
    },
    new MenuItem()
    {
        Shortcut = "o",
        MenuLabel = "Options",
        //To be implemented
    },
});


var userChoice = mainMenu.Run();