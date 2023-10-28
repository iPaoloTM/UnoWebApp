using Entities;

namespace UnoEngine;

public class NewValidator
{
    public bool ValidateMove(PlayerMove newAction, GameState tableState)
    {
        Card? lastCard;
        switch (newAction.PlayerAction)
        {
            
            //Check number and color of the card try1ing to be played
            case EPlayerAction.PlayCard:
            {
                lastCard = tableState.UsedDeck.First();

                if (newAction.PlayedCard is NumericCard numCard)
                {
                    if (lastCard is NumericCard lastCardNum)
                    {
                        return (numCard.Color == lastCardNum.Color || numCard.Number == lastCardNum.Number);
                    }
                    else if (lastCard is SpecialCard lastCardSpe)
                    {
                        return numCard.Color == lastCardSpe.Color;
                    }
                }

                else if (newAction.PlayedCard is SpecialCard speCard)
                {
                    return speCard.Color == lastCard.Color;
                }

                break;
            }
            
            //Check if the player has a playable card, if not, validate the move
            case EPlayerAction.Draw:
                lastCard = tableState.UsedDeck.First();
                return CanPlay(newAction, lastCard);
                break;
        }

        return false;
    }

    //Check if the player can play any of his cards on top of the one on the table
    public bool CanPlay(PlayerMove newAction, Card lastCardTable)
    {
        var playerHand = newAction.Player.HandCards;
        foreach (var card in playerHand)
        {
            if (lastCardTable is NumericCard numericLastCard)
            {
                if (card is NumericCard numericCard)
                {
                    if (numericCard.Color == numericLastCard.Color || numericCard.Number == numericLastCard.Number)
                    {
                        return true;
                    }
                }
                else if (card is SpecialCard specialCard)
                {
                    if (specialCard.Color == numericLastCard.Color) return true;
                }
            }
            else if (lastCardTable is SpecialCard specialLastCard)
            {
                if (card.Color == specialLastCard.Color) return true;
            }
        }

        return false;
    }
}