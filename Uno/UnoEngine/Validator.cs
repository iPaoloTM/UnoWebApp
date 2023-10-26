using Entities;

namespace UnoEngine;

public class Validator
    {
        public bool ValidateAction(PlayerMove previousAction, PlayerMove currentAction)
        {
            if (previousAction is null)
            {
                switch (previousAction)
                {
                    case EPlayerAction.PlayCard:
                        return ValidatePlayCard(currentAction);

                    case EPlayerAction.Draw:
                        return ValidateDraw(currentAction);

                    case EPlayerAction.NextPlayer:
                        return ValidateNextPlayer(currentAction);

                    case EPlayerAction.SaySomething:
                        return ValidateSaySomething(currentAction);

                    default:
                        return false;
                } 
            }
            else
            {
                switch (previousAction)
                {
                    case EPlayerAction.PlayCard:
                        return ValidatePlayCard(currentAction);

                    case EPlayerAction.Draw:
                        return ValidateDraw(currentAction);

                    case EPlayerAction.NextPlayer:
                        return ValidateNextPlayer(currentAction);

                    case EPlayerAction.SaySomething:
                        return ValidateSaySomething(currentAction);

                    default:
                        return false;
                } 
            }

        }

        private bool ValidatePlayCard(EPlayerAction currentAction)
        {
            switch (currentAction)
            {
                case EPlayerAction.PlayCard:
                    return false; 

                case EPlayerAction.Draw:
                    return false; 

                case EPlayerAction.NextPlayer:
                    return true; 

                case EPlayerAction.SaySomething:
                    return true;

                default:
                    return false;
            }
        }

        private bool ValidateDraw(EPlayerAction currentAction)
        {
            switch (currentAction)
            {
                case EPlayerAction.PlayCard:
                    return false; // PlayCard cannot follow Draw

                case EPlayerAction.Draw:
                    return true; // Draw can follow Draw

                case EPlayerAction.NextPlayer:
                    return true; // NextPlayer can follow Draw

                case EPlayerAction.SaySomething:
                    return true; // SaySomething can follow Draw

                default:
                    throw new ArgumentException("Invalid current player action.");
            }
        }

        private bool ValidateNextPlayer(EPlayerAction currentAction)
        {
            switch (currentAction)
            {
                case EPlayerAction.PlayCard:
                    return true; // PlayCard can follow NextPlayer

                case EPlayerAction.Draw:
                    return false; // Draw cannot follow NextPlayer

                case EPlayerAction.NextPlayer:
                    return false; // NextPlayer cannot follow NextPlayer

                case EPlayerAction.SaySomething:
                    return true; // SaySomething can follow NextPlayer

                default:
                    throw new ArgumentException("Invalid current player action.");
            }
        }

        private bool ValidateSaySomething(EPlayerAction currentAction)
        {
            switch (currentAction)
            {
                case EPlayerAction.PlayCard:
                    return true; // PlayCard can follow SaySomething

                case EPlayerAction.Draw:
                    return true; // Draw can follow SaySomething

                case EPlayerAction.NextPlayer:
                    return true; // NextPlayer can follow SaySomething

                case EPlayerAction.SaySomething:
                    return true; // SaySomething can follow SaySomething

                default:
                    throw new ArgumentException("Invalid current player action.");
            }
        }
    }
