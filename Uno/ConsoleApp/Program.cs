// See https://aka.ms/new-console-template for more information

using MenuSystem;
using UnoEngine;

//This isn't a good way to create the engine instance
GameEngine Game = new GameEngine(0);

string? startGame()
{
    GameMenu gameMenu = new GameMenu(Game);
    gameMenu.Draw();
    return null;
}

string? SetPlayerCount()
{
    Console.Write("Player count?");
    var countStr = Console.ReadLine()?.Trim();
    var count = int.Parse(countStr);
    
    //This is also kind of horrible code
    Game = new GameEngine(count);
    
    /**
    game.Players = new List<Player>();
    for (int i = 0; i < count; i++)
    {
        game.Players.Add(new Player()
        {
            NickName   = "Human " + i,
            PlayerType = EPlayerType.Human,
        });
    }*/
    return null;
}



string? runNewGameMenu()
{
    var startNewGameMenu = new Menu("New Game", new List<MenuItem>()
        {
            new MenuItem()
            {
                Shortcut = "c",
                MenuLabel = "Player count: ",
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
                MenuLabel = "Start the game of UNO", 
                MethodToRun = startGame
            },
        }
    );
    
    
    return startNewGameMenu.Run(EMenuLevel.Second);
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
    },
    new MenuItem()
    {
        Shortcut = "o",
        MenuLabel = "Options",
    },
});


var userChoice = mainMenu.Run();