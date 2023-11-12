namespace MenuSystem;

public class MenuNavigator
{
    public int SelectedOptionIndex { get; private set; } = 0;
    private int menuItemsCount;

    public MenuNavigator(int menuItemsCount)
    {
        this.menuItemsCount = menuItemsCount;
    }

    public int HandleKeyPress(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.DownArrow:
                SelectedOptionIndex = MoveSelectionDown();
                return SelectedOptionIndex;
            case ConsoleKey.UpArrow:
                SelectedOptionIndex = MoveSelectionUp();
                return SelectedOptionIndex;
        }

        return SelectedOptionIndex;
    }

    private int MoveSelectionUp()
    {
        SelectedOptionIndex = (SelectedOptionIndex - 1 + menuItemsCount) % menuItemsCount;
        if (SelectedOptionIndex < 0)
        {
            SelectedOptionIndex = menuItemsCount - 1;
        }

        return SelectedOptionIndex;
    }

    private int MoveSelectionDown()
    {
        SelectedOptionIndex = (SelectedOptionIndex + 1) % menuItemsCount;
        
        if (SelectedOptionIndex >= menuItemsCount)
        {
            SelectedOptionIndex = 0;
        }
        return SelectedOptionIndex;
    }

    public bool IsNavigationKey(ConsoleKey key)
    {
        return key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow;
    }
}
