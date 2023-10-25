using System.Diagnostics;

namespace Entities;

public class PlayerEdited
{
    public string Nickname { get; }
    public int Position { get; set; } = default!;
    public EPlayerType PlayerType { get; set; } // Right now hardcoded

    public List<Card> HandCards { get; set; } = new List<Card>();

    public PlayerEdited(string nickname)
    {
        Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname), "Nickname cannot be null.");
        HandCards = new List<Card>();
    }

    public List<ETurnResult> PlayTurn(PlayerTurn previousTurn)
    {
        List<ETurnResult> turnResults = new List<ETurnResult>();

        if (previousTurn.Card is NumericCard numericCard)
        {
            turnResults.AddRange(new[]
            {
                ETurnResult.PlayedCard,
                ETurnResult.ForcedDraw,
                ETurnResult.ForcedDrawPlay
            });
        }
        else if (previousTurn.Card is SpecialCard specialCard)
        {
            switch (specialCard.Effect)
            {
                case EEffect.DrawTwo:
                    turnResults.Add(ETurnResult.DrawTwoCards);
                    break;
                case EEffect.DrawFour:
                    turnResults.Add(ETurnResult.WildDrawFourCards);
                    //turnResults.Add(ETurnResult.ForcedDrawPlay);
                    break;
                case EEffect.Reverse:
                    turnResults.Add(ETurnResult.Reversed);
                    break;
                case EEffect.Skip:
                    turnResults.Add(ETurnResult.Skip);
                    break;
                case EEffect.Wild:
                    turnResults.Add(ETurnResult.PlayedCard);
            }
        }
        
    }

    public void TakeCards(CardDeck cardDeck, int count)
    { 
        var drawnCards = cardDeck.Draw(count);
        HandCards.AddRange(drawnCards!);
    }
    
 
    
    
}