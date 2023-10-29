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
                return CanPlayCard(newAction.PlayedCard, tableState);
                break;
            //Check if the player has a playable card, if not, validate the move
            case EPlayerAction.Draw:
                return !CanPlay(newAction, tableState);
                break;
            case EPlayerAction.NextPlayer:
                return true;
        }

        return false;
    }

    //Check if the player can play any of his cards on top of the one on the table
    public bool CanPlay(PlayerMove newAction, GameState state)
    {
        var playerHand = newAction.Player.HandCards;
        foreach (var card in playerHand)
        {
            if (CanPlayCard(card, state)) return true;
        }

        return false;
    }

    public bool CanPlayCard(Card card, GameState state)
    {
        if (card.Color == EColors.Black) return true;

        if (state.UsedDeck.First() is NumericCard numericLastCard)
        {
            if (card is NumericCard numericCard)
            {
                return (numericCard.Color == numericLastCard.Color || numericCard.Number == numericLastCard.Number);
            }
            else if (card is SpecialCard specialCard)
            {
                return (specialCard.Color == numericLastCard.Color);
            }
        }
        else if (state.UsedDeck.First() is SpecialCard specialLastCard)
        {
            if (card is SpecialCard cardSpe)
            {
                return (cardSpe.Color == specialLastCard.Color || cardSpe.Effect == specialLastCard.Effect ||
                        (specialLastCard.Color == EColors.Black && state.ColorInPlay == cardSpe.Color));
            }
            else if (card is NumericCard cardNum)
            {
                return (cardNum.Color == specialLastCard.Color ||
                        (specialLastCard.Color == EColors.Black && state.ColorInPlay == cardNum.Color));
            }
        }

        return false;
    }
}