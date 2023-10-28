using System.Diagnostics;
using Entities;

namespace UnoEngine;

public class Validator
{
    /*
    public Boolean ValidatePlayerMove(PlayerMove previousPlayer, PlayerMove currentPlayer)
    {
        if (previousPlayer.PlayedCard is NumericCard)
        {
            return ValidateByCard(previousPlayer, currentPlayer, previousPlayer.PlayedCard);
        }
        else if (previousPlayer.PlayedCard is SpecialCard { Effect: EEffect.Wild })
        {
            return ValidateByCard(previousPlayer, currentPlayer, previousPlayer.PlayedCard);
        }
        else if (previousPlayer.PlayedCard is SpecialCard { Effect: EEffect.Reverse })
        {
            return ValidateByCard(previousPlayer, currentPlayer, previousPlayer.PlayedCard);
        }
        else if (previousPlayer.PlayedCard is SpecialCard { Effect: EEffect.Skip })
        {
            return currentPlayer.PlayerAction switch
            {
                EPlayerAction.Draw => false,
                EPlayerAction.PlayCard => false,
                EPlayerAction.NextPlayer => true,
                EPlayerAction.SaySomething => true,
                _ => false
            };
        }
        else if (previousPlayer.PlayedCard is SpecialCard { Effect: EEffect.DrawTwo } )
        {
            if (currentPlayer.PlayerPreviousMove == null)
            {
                return currentPlayer.PlayerAction switch
                {
                    EPlayerAction.Draw => true,
                    EPlayerAction.PlayCard => false,
                    EPlayerAction.NextPlayer => false,
                    EPlayerAction.SaySomething => true,
                    _ => false
                };  
            }
            if (currentPlayer.PlayerPreviousMove.PlayerAction == EPlayerAction.Draw && currentPlayer.PlayerPreviousMove.PlayerPreviousMove == null )
            {
                return currentPlayer.PlayerAction switch
                {
                    EPlayerAction.Draw => true,
                    EPlayerAction.PlayCard => false,
                    EPlayerAction.NextPlayer => false,
                    EPlayerAction.SaySomething => true,
                    _ => false
                };  
            }
            if (currentPlayer.PlayerPreviousMove.PlayerAction == EPlayerAction.Draw &&
                (currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                 currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.SaySomething))
            {
                return currentPlayer.PlayerAction switch
                {
                    EPlayerAction.Draw => false,
                    EPlayerAction.PlayCard => false,
                    EPlayerAction.NextPlayer => true,
                    EPlayerAction.SaySomething => true,
                    _ => false
                };
            }
            if (currentPlayer.PlayerPreviousMove?.PlayerAction == EPlayerAction.SaySomething &&
                (currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                 currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw))
            {
                return currentPlayer.PlayerAction switch
                {
                    EPlayerAction.Draw => false,
                    EPlayerAction.PlayCard => false,
                    EPlayerAction.NextPlayer => true,
                    EPlayerAction.SaySomething => true,
                    _ => false
                };
            }
        }
        else if (previousPlayer.PlayedCard is SpecialCard { Effect: EEffect.DrawFour } )
        {
            if (currentPlayer.PlayerPreviousMove == null)
            {
                return currentPlayer.PlayerAction switch
                {
                    EPlayerAction.Draw => true,
                    EPlayerAction.PlayCard => false,
                    EPlayerAction.NextPlayer => false,
                    EPlayerAction.SaySomething => true,
                    _ => false
                };  
            }
            if (currentPlayer.PlayerPreviousMove.PlayerAction == EPlayerAction.Draw && currentPlayer.PlayerPreviousMove.PlayerPreviousMove == null )
            {
                return currentPlayer.PlayerAction switch
                {
                    EPlayerAction.Draw => true,
                    EPlayerAction.PlayCard => false,
                    EPlayerAction.NextPlayer => false,
                    EPlayerAction.SaySomething => true,
                    _ => false
                };  
            }
            if (currentPlayer.PlayerPreviousMove.PlayerAction == EPlayerAction.Draw &&
                currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw)
            {
                return currentPlayer.PlayerAction switch
                {
                    EPlayerAction.Draw => true,
                    EPlayerAction.PlayCard => false,
                    EPlayerAction.NextPlayer => false,
                    EPlayerAction.SaySomething => true,
                    _ => false
                };
            }

            if (currentPlayer.PlayerPreviousMove?.PlayerAction == EPlayerAction.SaySomething &&
                (currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                 currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction ==
                 EPlayerAction.Draw))
            {
                return currentPlayer.PlayerAction switch
                {
                    EPlayerAction.Draw => true,
                    EPlayerAction.PlayCard => false,
                    EPlayerAction.NextPlayer => false,
                    EPlayerAction.SaySomething => true,
                    _ => false
                };
            }

            if (currentPlayer.PlayerPreviousMove?.PlayerAction == EPlayerAction.SaySomething &&
                currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                 currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw)
            {
                return currentPlayer.PlayerAction switch
                {
                    EPlayerAction.Draw => true,
                    EPlayerAction.PlayCard => false,
                    EPlayerAction.NextPlayer => false,
                    EPlayerAction.SaySomething => true,
                    _ => false
                };
            }
            if (currentPlayer.PlayerPreviousMove?.PlayerAction == EPlayerAction.SaySomething &&
                currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw)
            {
                return currentPlayer.PlayerAction switch
                {
                    EPlayerAction.Draw => true,
                    EPlayerAction.PlayCard => false,
                    EPlayerAction.NextPlayer => true,
                    EPlayerAction.SaySomething => true,
                    _ => false
                };
            }
            if (currentPlayer.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw &&
                currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.SaySomething)
            {
                return currentPlayer.PlayerAction switch
                {
                    EPlayerAction.Draw => true,
                    EPlayerAction.PlayCard => false,
                    EPlayerAction.NextPlayer => true,
                    EPlayerAction.SaySomething => true,
                    _ => false
                };
            }
        }
        return false;
    }
    
    public bool ValidateByCard(PlayerMove previousPlayer, PlayerMove currentPlayer, Card card)
    {
        if (currentPlayer.PlayerPreviousMove is null)
        {
            return currentPlayer.PlayerAction switch
            {
                EPlayerAction.Draw => true,
                EPlayerAction.PlayCard => true,
                EPlayerAction.NextPlayer => false,
                EPlayerAction.SaySomething => true,
                _ => false
            };
        }
        else if (currentPlayer.PlayerPreviousMove.PlayerAction == EPlayerAction.Draw)
        {
            return currentPlayer.PlayerAction switch
            {
                EPlayerAction.Draw => false,
                EPlayerAction.PlayCard => true,
                EPlayerAction.NextPlayer => true,
                EPlayerAction.SaySomething => true,
                _ => false
            };
        }
        else if (currentPlayer.PlayerPreviousMove.PlayerAction == EPlayerAction.PlayCard)
        {
            return currentPlayer.PlayerAction switch
            {
                EPlayerAction.Draw => false,
                EPlayerAction.PlayCard => false,
                EPlayerAction.NextPlayer => true,
                EPlayerAction.SaySomething => true,
                _ => false
            };
        }
        else if (currentPlayer.PlayerPreviousMove.PlayerAction == EPlayerAction.SaySomething &&
            (currentPlayer.PlayerPreviousMove.PlayerPreviousMove is null ||
            currentPlayer.PlayerPreviousMove.PlayerPreviousMove.PlayerAction == EPlayerAction.NextPlayer))
        {
            return currentPlayer.PlayerAction switch
            {
                EPlayerAction.Draw => true,
                EPlayerAction.PlayCard => true,
                EPlayerAction.NextPlayer => false,
                EPlayerAction.SaySomething => true,
                _ => false
            };
        }
        else if (currentPlayer.PlayerPreviousMove.PlayerAction == EPlayerAction.SaySomething &&
            currentPlayer.PlayerPreviousMove?.PlayerPreviousMove?.PlayerAction == EPlayerAction.Draw)
        {
            return currentPlayer.PlayerAction switch
            {
                EPlayerAction.Draw => false,
                EPlayerAction.PlayCard => true,
                EPlayerAction.NextPlayer => true,
                EPlayerAction.SaySomething => true,
                _ => false
            };
        }
        else if (currentPlayer.PlayerPreviousMove is { PlayerAction: EPlayerAction.SaySomething, PlayerPreviousMove.PlayerAction: EPlayerAction.PlayCard })
        {
            return currentPlayer.PlayerAction switch
            {
                EPlayerAction.Draw => false,
                EPlayerAction.PlayCard => false,
                EPlayerAction.NextPlayer => true,
                EPlayerAction.SaySomething => true,
                _ => false
            };
        }
        else if (currentPlayer.PlayerPreviousMove?.PlayerAction == EPlayerAction.NextPlayer &&
            (currentPlayer.PlayerPreviousMove.PlayerPreviousMove is null ||
            currentPlayer.PlayerPreviousMove.PlayerPreviousMove.PlayerAction == EPlayerAction.NextPlayer))
        {
            return currentPlayer.PlayerAction switch
            {
                EPlayerAction.Draw => true,
                EPlayerAction.PlayCard => true,
                EPlayerAction.NextPlayer => false,
                EPlayerAction.SaySomething => true,
                _ => false
            };
        }
        return false;
        
    }
    */
}
