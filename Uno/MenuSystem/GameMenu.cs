using System.Net;
using System.Text.Json;
using Entities;
using UnoEngine;

namespace MenuSystem;

using UnoEngine;
using System.Threading;

public class GameMenu
{
    private int _selectedOptionIndex = 0;
    public MenuNavigator GameOptionsNavigator { get; set; }

    public GameMenu(GameEngine game)
    {
        this.Game = game;
        GameOptionsNavigator = new MenuNavigator(4);
    }

    public GameEngine Game { get; set; }
    private Player CurrPlayer { get; set; } = default!;

    public void Run()
    {
        do
        {
            Console.Clear();
            Game.NewTurn();

            CurrPlayer = Game.State.Players[Game.State.ActivePlayerNo];
            Console.WriteLine("--- " + CurrPlayer.Nickname + "'S TURN ---");
            Console.ReadLine();
            Console.Clear();

            DrawMenu();

            //Print card information of current player
            if (CurrPlayer.PlayerType == EPlayerType.Human)
            {
                ShowHand();
            }

            // Ask the player for his choice
            PlayerPrompt();
        } while (!Game.State.GameOver);

        Console.Clear();
        Console.WriteLine("> G A M E  O V E R <");
        Console.WriteLine("Congratulations!");
        Console.WriteLine("Player: " + CurrPlayer.Nickname + " has won the game!");
        Console.ReadLine();
    }

    public void AIturn()
    {
        Console.WriteLine("AI is deciding..");

        Console.ReadLine();

        var aiPlayNumber = Game.State.ActivePlayerNo;
        var cardsCount = Game.State.Players[aiPlayNumber].HandCards.Count;

        int code = Game.AIplay();


        if (Game.State.Players[aiPlayNumber].HandCards.Count > cardsCount)
        {
            Console.WriteLine("AI drew a card.");
        }
        else
        {
            var lastCard = Game.State.UsedDeck.First();

            ConsoleColor color = ColorConverter(lastCard.Color);

            Console.ForegroundColor = color;
            if (Game.State.Players[aiPlayNumber].HandCards.Count == cardsCount)
            {
                Console.WriteLine("AI drew a card.");
            }

            switch (lastCard)
            {
                case NumericCard numCard:
                    Console.WriteLine("AI has played: " + numCard.Color + " " + numCard.Number);
                    break;
                case SpecialCard speCard:
                    Console.WriteLine("AI has played: " + speCard.Color + " " + speCard.Effect);
                    break;
            }
        }

        Console.ReadLine();

        Console.ResetColor();

        DrawMenu();
        if (CurrPlayer.PlayerType == EPlayerType.Human)
        {
            ShowHand();
        }
    }


    //ASK THE PLAYER FOR HIS ACTIONS IN THIS FUNCTION!!
    public void PlayerPrompt()
    {
        string? screamingPlayer = null;
        do //While the player hasn't skipped...
        {
            if (CurrPlayer.PlayerType == EPlayerType.AI)
            {
                AIturn();
            }
            else
            {
                var promptsList =
                    new List<string> { "1. Play a card ", "2. Draw from deck ", "3. Say something", "4. Skip " };
                Console.WriteLine("Choose an action: ");
                for (int i = 0; i < promptsList.Count; i++)
                {
                    if (i == _selectedOptionIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }

                    Console.WriteLine(promptsList[i]);

                    if (i == _selectedOptionIndex)
                    {
                        Console.ResetColor();
                    }
                }

                var choice = Console.ReadKey();
                int success = 0;
                if (GameOptionsNavigator.IsNavigationKey(choice.Key))
                {
                    _selectedOptionIndex = GameOptionsNavigator.HandleKeyPress(choice);
                    // Redraw menu with the new selected option
                    Console.Clear();
                    DrawMenu();
                    ShowHand();
                    continue; // Skip the rest of the loop to immediately handle the next keypress
                }
                else if (choice.Key == ConsoleKey.Enter)
                {
                    switch (_selectedOptionIndex + 1)
                    {
                        // Try to play a card
                        case 1:
                            if (Game.State.TurnOver)
                            {
                                Console.WriteLine("Y" +
                                                  "ou already acted this turn!");
                                Console.ReadLine();
                            }
                            else
                            {
                                var maxCards = CurrPlayer.HandCards.Count;
                                int chosenCard =
                                    PromptValidator.UserPrompt("Choose a card from 1 to " + maxCards + ": ", 0,
                                        maxCards);
                                if (chosenCard != -1)
                                {
                                    var playingCard = CurrPlayer.HandCards[chosenCard - 1];
                                    var movePlay =
                                        new PlayerMove(CurrPlayer, EPlayerAction.PlayCard, playingCard);
                                    success = Game.HandlePlayerAction(movePlay);
                                    if (success == 2)
                                    {
                                        //Player needs to choose a color
                                        Console.WriteLine("Choose a new color: ");
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("1) Red");
                                        Console.ForegroundColor = ConsoleColor.Blue;
                                        Console.WriteLine("2) Blue");
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine("3) Yellow");
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("4) Green");
                                        Console.ResetColor();

                                        int color = PromptValidator.UserPrompt("", 0, 4);
                                        Game.SetColorInPlay(color);
                                    }
                                    else if (success != 1 && success != 4)
                                    {
                                        Console.WriteLine("Can't play the selected card!");
                                        Console.ReadLine();
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid option");
                                    Console.ReadLine();
                                }
                            }

                            Console.Clear();
                            DrawMenu();
                            ShowHand();
                            break;
                        //Try to draw a card from the game deck
                        case 2:
                            if (Game.State.TurnOver)
                            {
                                Console.WriteLine("You already acted this turn!");
                                Console.ReadLine();
                            }
                            else if (!Game.State.CanDraw)
                            {
                                Console.WriteLine("You can play one of your cards!");
                                Console.ReadLine();
                            }
                            else
                            {
                                //The player wants to draw a card
                                //Card should be null here??
                                var moveDraw = new PlayerMove(CurrPlayer, EPlayerAction.Draw, null);
                                success = Game.HandlePlayerAction(moveDraw);
                                if (success != 3 && success != 1)
                                {
                                    Console.WriteLine("You can play one of your cards!");
                                    Console.ReadLine();
                                }
                            }

                            Console.Clear();
                            DrawMenu();
                            ShowHand();
                            break;
                        //Say something (uno)
                        case 3:
                            screamingPlayer = PromptValidator.UserPrompt("What do you want to say?");
                            Game.HandleUnoShouting(CurrPlayer, screamingPlayer);
                            Game.HandleUnoReporting(screamingPlayer);
                            Console.Clear();
                            DrawMenu();
                            ShowHand();
                            break;
                        //PLayer wants to end his turn 
                        case 4:
                            //Only end turn if the player has drawn or played
                            var moveSkip = new PlayerMove(CurrPlayer, EPlayerAction.NextPlayer, null);
                            int overSuccess = Game.HandlePlayerAction(moveSkip);
                            if (overSuccess != 1)
                            {
                                Console.WriteLine("Can't end turn without doing an action");
                                Console.ReadLine();
                                Console.Clear();
                                DrawMenu();
                                ShowHand();
                            }

                            break;

                        default:
                            Console.WriteLine("Invalid option");
                            Console.ReadLine();
                            Console.Clear();
                            DrawMenu();
                            ShowHand();
                            break;
                    }
                }
            }
        } while (!Game.State.EndTurn);
    }

    public void ShowHand()
    {
        Console.WriteLine("Your Hand:");
        int i = 0;
        //Add colored text in the future
        foreach (Card c in CurrPlayer.HandCards)
        {
            ConsoleColor color = ColorConverter(c.Color);

            Console.ForegroundColor = color;
            if (c is NumericCard)
            {
                NumericCard nc = (NumericCard)c;
                Console.Write(i + 1 + ") " + nc.Color + " " + nc.Number + ", ");
            }
            else if (c is SpecialCard)
            {
                SpecialCard nc = (SpecialCard)c;
                Console.Write(i + 1 + ") " + nc.Color + " " + nc.Effect + ", ");
            }

            Console.ResetColor();

            i++;
        }

        Console.WriteLine();
    }

    public void ShowLastCard()
    {
        var lastCard = Game.State.UsedDeck.First();

        ConsoleColor color = ColorConverter(lastCard.Color);

        Console.ForegroundColor = color;

        switch (lastCard)
        {
            case NumericCard numCard:
                Console.WriteLine("Last played card: " + numCard.Color + " " + numCard.Number);
                break;
            case SpecialCard speCard:
                Console.WriteLine("Last played card: " + speCard.Color + " " + speCard.Effect);
                break;
        }

        Console.ResetColor();


        if (Game.State.UsedDeck.Cards.First().Color == EColors.Black)
        {
            Console.ForegroundColor = ColorConverter(Game.State.ColorInPlay);
            Console.WriteLine("Chosen color: " + Game.State.ColorInPlay);
            Console.ResetColor();
        }

        Console.WriteLine();
    }

    public void DrawMenu()
    {
        //Display whose turn it is
        Console.WriteLine("Player " + CurrPlayer.Nickname + "'s turn");
        Console.WriteLine("================================================");

        int a = 0;
        //Print the number of cards each player has
        foreach (Player plyr in Game.State.Players)
        {
            Console.Write(a + 1 + ". " + plyr.Nickname + " - ");
            foreach (Card c in plyr.HandCards)
            {
                //Print a # for each card
                Console.Write("#");
            }

            Console.WriteLine();
            a++;
        }

        //Show the last card of the used deck
        ShowLastCard();
    }

    private ConsoleColor ColorConverter(EColors c)
    {
        ConsoleColor color = ConsoleColor.White;
        switch (c)
        {
            case EColors.Red:
                color = ConsoleColor.Red;
                break;
            case EColors.Yellow:
                color = ConsoleColor.Yellow;
                break;
            case EColors.Green:
                color = ConsoleColor.Green;
                break;
            case EColors.Blue:
                color = ConsoleColor.Blue;
                break;
            case EColors.Black:
                color = ConsoleColor.Black;
                break;
            default:
                color = ConsoleColor.White;
                break;
        }

        return color;
    }
}