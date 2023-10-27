namespace Entities;

public class CardDeck : Deck
{
    
    public CardDeck()
    {
        //Initialize Numeric Cards 
        EColors color;
        var gameDeck = new List<Card>();
        for (int c = 0; c < 4; c++)
        {
            switch (c)
            {
                case 0:
                    color = EColors.Red;
                    break;
                case 1:
                    color = EColors.Blue;
                    break;
                case 2:
                    color = EColors.Green;
                    break;
                case 3:
                    color = EColors.Yellow;
                    break;
                default:
                    color = EColors.Red;
                    break;
            }

            gameDeck.Add(new NumericCard()
            {
                Number = (ENumbers)0,
                Color = color,
            });

            for (int j = 0; j < 2; j++)
            {
                for (int i = 1; i < 13; i++)
                {
                    if (i < 10)
                    {
                        gameDeck.Add(new NumericCard()
                        {
                            Number = (ENumbers)i,
                            Color = color,
                        });

                    }
                    else
                    {
                        //Initialize Special Cards
                        EEffect effect;
                        switch (i)
                        {
                            case 10:
                                effect = EEffect.Skip;
                                break;
                            case 11:
                                effect = EEffect.Reverse;
                                break;
                            case 12:
                                effect = EEffect.DrawTwo;
                                break;
                            default:
                                effect = EEffect.DrawTwo;
                                break;
                        }

                        gameDeck.Add(new SpecialCard()
                        {
                            Effect = effect,
                            Color = color,
                        });

                    }

                }

                for (int i = 0; i < 2; i++)
                {
                    gameDeck.Add(new SpecialCard()
                    {
                        Effect = EEffect.DrawFour,
                        Color = EColors.Black,
                    });
                    gameDeck.Add(new SpecialCard()
                    {
                        Effect = EEffect.Wild,
                        Color = EColors.Black,
                    });
                }
            }
        }
        Cards = gameDeck;
    }

    public CardDeck(List<Card> cards)
    {
        Cards = cards;
    }

    public List<Card>? Draw(int count) //thing about this nullable reference
    {
        var drawnCards = Cards.Take(count).ToList();
        Cards.RemoveAll(x => drawnCards.Contains(x));
        return drawnCards;
    }
}