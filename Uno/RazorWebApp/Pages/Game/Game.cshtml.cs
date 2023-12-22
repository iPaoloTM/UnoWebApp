using DAL;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;

namespace RazorWebApp.Pages.Game;

public class Game : PageModel
{
    private readonly GameRepositoryEF _gameRepository = default!;
    public GameEngine Engine;
    
    

    [BindProperty] public bool IsPlayerTurn { get; set; } = false;
    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }

    [BindProperty(SupportsGet = true)] public int PlayerId { get; set; }

    public Game(GameRepositoryEF gameRepository)
    {
        _gameRepository = gameRepository;
        Engine = new GameEngine(_gameRepository);
    }


    public IActionResult? OnGet(Guid gameId)
    {
        var gameState = _gameRepository.LoadGame(GameId);
        Engine.State = gameState;
        if (PlayerId == Engine.State.ActivePlayerNo) IsPlayerTurn = true;

        if (Engine.State.GameOver)
        {
            return RedirectToPage("../EndGame/EndGame", new { GameId = gameId, WinPlayer =  Engine.State.Players[Engine.State.ActivePlayerNo].Nickname});
        }

        return null;
    }

    public IActionResult OnPostCardClicked(Guid gameId, int cardIndex, int currPlayer)
    {
        var currState = _gameRepository.LoadGame(gameId);
        Engine.State = currState;
        Console.WriteLine("Chosen card is " + cardIndex);

        if (!Engine.State.TurnOver)
        {
            var playingCard = Engine.State.Players[Engine.State.ActivePlayerNo].HandCards[cardIndex];
            Console.WriteLine(playingCard);
            var movePlay =
                new PlayerMove(Engine.State.Players[Engine.State.ActivePlayerNo], EPlayerAction.PlayCard, playingCard);
            int success = Engine.HandlePlayerAction(movePlay);
            Console.WriteLine(success);
            if (success == 1)
            {
                Console.WriteLine("SAVING...");
                _gameRepository.Save(gameId, Engine.State);
            } else if (success == 2)
            {
                //User has to choose color
                Console.WriteLine("COLOR Reached");
                TempData["showColorSelection"] = true;
                _gameRepository.Save(gameId, Engine.State);
            }  else if (success == 4)
            {
                //User has won
                Console.WriteLine("Game over!");
                TempData["WinNickname"] = Engine.State.Players[currPlayer].Nickname;
                _gameRepository.Save(gameId, Engine.State);
                Console.WriteLine("!!!!!!!!!"+Engine.State.GameOver);
                return RedirectToPage("../EndGame/EndGame", new { GameId = gameId, WinPlayer =  Engine.State.Players[currPlayer].Nickname});
            }   
        }
        return RedirectToPage("../Game/Game", new { GameId = gameId, PlayerId = currPlayer });
    }

    public IActionResult  OnPostSelectColor(string selectedColor, Guid gameId, int currPlayer)
    {
        var currState = _gameRepository.LoadGame(gameId);
        Engine.State = currState;
        switch (selectedColor)
        {
            case "yellow":
                Console.WriteLine("YELLOW" +selectedColor);
                Engine.SetColorInPlay(3);
                break;
            case "blue":
                Console.WriteLine("BLUE " +selectedColor);
                Engine.SetColorInPlay(2);
                break;
            case "red":
                Console.WriteLine("RED " +selectedColor);
                Engine.SetColorInPlay(1);
                break;
            case "green":
                Console.WriteLine("GREEN " +selectedColor);
                Engine.SetColorInPlay(4);
                break;
        }
        
        _gameRepository.Save(gameId, Engine.State);
        return RedirectToPage("../Game/Game", new { GameId = gameId, PlayerId = currPlayer });
        
    }

    public IActionResult OnPostDrawCard(Guid gameId, int currPlayer)
    {
        /* TODO
         For some reason i cannot comprehend, if I dont load the state every time from the
         database the engine accessed is a new instance without the relevant game information
         I have no clue how to fix this
         */
        var currState = _gameRepository.LoadGame(gameId);
        Engine.State = currState;
        if (!Engine.State.TurnOver && Engine.State.CanDraw)
        {
            var moveDraw = new PlayerMove(Engine.State.Players[Engine.State.ActivePlayerNo], EPlayerAction.Draw, null);
            Engine.HandlePlayerAction(moveDraw);
            _gameRepository.Save(gameId, Engine.State);
        }
        //Console.WriteLine(Engine.State.Id);

        return RedirectToPage("../Game/Game", new { GameId = gameId, PlayerId = currPlayer });
    }


    public IActionResult OnPostSkipTurn(Guid gameId, int currPlayer)
    {
        var currState = _gameRepository.LoadGame(GameId);
        Engine.State = currState;

        if (Engine.State.TurnOver)
        {
            var moveSkip = new PlayerMove(Engine.State.Players[currPlayer], EPlayerAction.NextPlayer,
                null);
            var response = Engine.HandlePlayerAction(moveSkip);

            //Conditions to end turn are met
            if (response == 1)
            {
                Engine.NewTurn();
                _gameRepository.Save(gameId, Engine.State);
            }
        }

        return RedirectToPage("../Game/Game", new { GameId = gameId, PlayerId = currPlayer });
    }

    public IActionResult OnPostShout(string? shoutingText, Guid gameId, int currPlayer)
    {
        var currState = _gameRepository.LoadGame(GameId);
        Engine.State = currState;
        Player player = Engine.State.Players[currPlayer];
        Engine.HandleUnoShouting(player, shoutingText);
        Engine.HandleUnoReporting(shoutingText);
        _gameRepository.Save(gameId, Engine.State);
        return RedirectToPage("../Game/Game", new { GameId = gameId, PlayerId = currPlayer });

    }

    public IActionResult OnPostAIPlay(Guid gameId, int currPlayer)
    {
        var currState = _gameRepository.LoadGame(gameId);
        Engine.State = currState;
        if (Engine.State.Players[currPlayer].PlayerType == EPlayerType.AI)
        {
            Engine.AIplay();
        }
        Engine.NewTurn();
        _gameRepository.Save(gameId,Engine.State);
        return RedirectToPage("../Game/Game", new { GameId = gameId, PlayerId = currPlayer });
 
    }
}