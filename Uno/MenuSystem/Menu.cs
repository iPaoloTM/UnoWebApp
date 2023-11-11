using DAL;

namespace MenuSystem;
using UnoEngine;

public class Menu
{
    public string? Title { get; set; }
    public Dictionary<string, MenuItem> MenuItems { get; set; } = new();

    private const string MenuSeparator = "=======================";
    private static readonly HashSet<string> ReservedShortcuts = new() {"x", "b", "r"};

    private int selectedOptionIndex = 0;
    
    public Menu(string? title, List<MenuItem> menuItems)
    {
        Title = title;
        foreach (var menuItem in menuItems)
        {
            if (ReservedShortcuts.Contains(menuItem.Shortcut.ToLower()))
            {
                throw new ApplicationException(
                    $"Menu shortcut '{menuItem.Shortcut.ToLower()}' in not allowed list!");
            }

            if (MenuItems.ContainsKey(menuItem.Shortcut.ToLower()))
            {
                throw new ApplicationException(
                    $"Menu shortcut '{menuItem.Shortcut.ToLower()}' is already registered!");
            }

            MenuItems[menuItem.Shortcut.ToLower()] = menuItem;
        }
    }

    private void Draw(EMenuLevel menuLevel)
    {
        Console.Clear();
        if (!string.IsNullOrWhiteSpace(Title))
        {
            Console.WriteLine(Title);
            Console.WriteLine(MenuSeparator);
        }

        for (int i = 0; i < MenuItems.Count; i++)
        {
            if (i == selectedOptionIndex)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.Write(MenuItems.ElementAt(i).Key);
            Console.Write(") ");
            Console.WriteLine(MenuItems.ElementAt(i).Value.MenuLabel);

            if (i == selectedOptionIndex)
            {
                Console.ResetColor();
            }
        }

        if (menuLevel != EMenuLevel.First)
        {
            Console.Write("b) ");
            Console.Write("Back");
            Console.WriteLine();
        }

        if (menuLevel == EMenuLevel.Other)
        {
            Console.Write("r) ");
            Console.Write("Return to main");
            Console.WriteLine();
        }

        Console.Write("x) ");
        Console.Write("eXit");

        //Console.WriteLine(MenuSeparator);
        //Console.Write("Your choice:");
    }

    public string Run(EMenuLevel menuLevel = EMenuLevel.First)
    {
        Console.Clear();
        do
        {
            Draw(menuLevel);

            var key = Console.ReadKey().Key;
            HandleKeyPress(key);

            if (key == ConsoleKey.Enter)
            {
                var selectedShortcut = MenuItems.ElementAt(selectedOptionIndex).Key;
                if (MenuItems[selectedShortcut].SubMenuToRun != null)
                {
                    var result = MenuItems[selectedShortcut].SubMenuToRun!(menuLevel == EMenuLevel.First
                        ? EMenuLevel.Second
                        : EMenuLevel.Other);
                    // TODO: Handle result - b, x, r
                }
                else if (MenuItems[selectedShortcut].MethodToRun != null)
                {
                    var result = MenuItems[selectedShortcut].MethodToRun!();
                    if (result?.ToLower() == "x")
                    {
                        return "x";
                    }
                }
            }

            Console.WriteLine();
        } while (true);
    }

    private void HandleKeyPress(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.DownArrow:
                MoveSelectionDown();
                break;
            case ConsoleKey.UpArrow:
                MoveSelectionUp();
                break;
        }
    }

    private void MoveSelectionUp()
    {
        selectedOptionIndex = (selectedOptionIndex - 1 + MenuItems.Count) % (MenuItems.Count+1);
        if (selectedOptionIndex < 0)
        {
            selectedOptionIndex = MenuItems.Count - 1;
        }
    }

    private void MoveSelectionDown()
    {
        selectedOptionIndex = (selectedOptionIndex + 1) % (MenuItems.Count+1);
        if (selectedOptionIndex >= MenuItems.Count)
        {
            selectedOptionIndex = 0;
        }
    }

}
