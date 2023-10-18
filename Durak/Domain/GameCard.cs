namespace Domain;

public class GameCard
{
    public ECardSuite CardSuite { get; set; }
    public ECardValue CardValue { get; set; }

    public override string ToString()
    {
        return CardSuiteToString + CardValue.ToString();
    }

    private string CardSuiteToString() =>
        CardSuite switch
        {
            ECardSuite.Clubs => "♣️",
            ECardSuite.Spades => "♠️",
            ECardSuite.Diamonds => "♦️",
            ECardSuite.Hearts => "❤️",
            _ => "-"
        };
    
    
}