using Entities;

namespace MenuSystem;

public class OptionsMenu
{
  public GameOptions Run()
  {
    var moddedOptions = new GameOptions();
    var userInput = "-1";
    do
    {
      Console.Clear();
      Console.WriteLine("a) Change hand size. Current size: "+ moddedOptions.HandSize);
      Console.WriteLine("b) Change order. Currently ascending is "+moddedOptions.InitialOrder);
      Console.WriteLine("x) Go back");
      userInput = PromptValidator.UserPrompt("Choose an option:");
      switch (userInput)
      {
        case "a":
          Console.WriteLine("Enter new hand size: ");
          var newHSize = PromptValidator.UserPrompt("Enter new hand size [2-9]: ", 2, 9);
          if (newHSize != -1) moddedOptions.HandSize = newHSize;
          break;
        case "b":
          moddedOptions.InitialOrder = !moddedOptions.InitialOrder;
          break;
      }
      Console.Clear();
    } while (userInput != "x");

    return moddedOptions;
  }

}