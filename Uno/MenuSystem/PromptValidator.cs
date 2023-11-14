namespace MenuSystem;

public class PromptValidator
{
    public static string? UserPrompt(string prompt)
    {
        if (prompt != "") Console.WriteLine(prompt);
        return Console.ReadLine();
    }

    public static int UserPrompt(string prompt, int minRange, int maxRange)
    {
        if (prompt != "") Console.WriteLine(prompt);
        var userInput = Console.ReadLine();
        if (int.TryParse(userInput, out int chosenInt) && !(chosenInt > maxRange || chosenInt < minRange))
        {
            return chosenInt;
        }
        return -1;
    }
}