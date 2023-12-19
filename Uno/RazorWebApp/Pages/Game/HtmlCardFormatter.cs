using Entities;

namespace RazorWebApp.Pages.Game;

public static class HtmlCardFormatter
{

    public static String Convert(Card card)
    {
        String color = ".";
        
        switch (card.Color)
        {
            case (EColors.Red):
                color = "red";
                break;
            case (EColors.Blue):
                color = "blue";
                break;
            case (EColors.Yellow):
                color = "yellow";
                break;
            case (EColors.Green):
                color = "green";
                break;
            case (EColors.Black):
                color = "black";
                break;
            default:
                color = "black";
                break;
        }
        if (card is NumericCard)
        {
            NumericCard c = (NumericCard)card;

            ENumbers n = c.Number;
            String number = ".";

            switch (n)
            {
                case ENumbers.Zero:
                    number = "number-0";
                    break;
                case ENumbers.One:
                    number = "number-1";
                    break;
                case ENumbers.Two:
                    number = "number-2";
                    break;
                case ENumbers.Three:
                    number = "number-3";
                    break;
                case ENumbers.Four:
                    number = "number-4";
                    break;
                case ENumbers.Five:
                    number = "number-5";
                    break;
                case ENumbers.Six:
                    number = "number-6";
                    break;
                case ENumbers.Seven:
                    number = "number-7";
                    break;
                case ENumbers.Eight:
                    number = "number-8";
                    break;
                case ENumbers.Nine:
                    number = "number-9";
                    break;
                default:
                    number = "number-0";
                    break;
            }

            return "<div class=\"uno-card "+color+" "+number+"\">\n    " +
                   "<div class=\"uno-card-body\">\n      " +
                   "<div class=\"uno-card-number\">" +
                   "</div>\n" +
                   "</div>\n" +
                   "</div>";

        }
        else
        {
            SpecialCard c = (SpecialCard)card;
            EEffect f = c.Effect;
            String effect = ".";

            switch (f)
            {
                case EEffect.Reverse:
                    effect = "reverse";
                    break;
                case EEffect.Wild:
                    effect = "wild";
                    break; 
                case EEffect.DrawFour:
                    effect = "draw-4";
                    break;
                case EEffect.DrawTwo:
                    effect = "draw-2";
                    break;
                case EEffect.Skip:
                    effect = "skip";
                    break;
                default:
                    effect = "draw-2";
                    break;
            }

            return "<div class=\"uno-card " + color + " " + effect + "\">\n" +
                   "<div class=\"uno-card-body\">\n" +
                   "<div class=\"uno-card-number\">" +
                   "</div>\n" +
                   "</div>\n" +
                   "</div>\n";

        }

        return "null";
    }
}