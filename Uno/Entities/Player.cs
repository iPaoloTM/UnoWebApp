// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using Entities;

public class Player
{
    public string Nickname { get; set; } = default!;
    public EPlayerType PlayerTipe { get; set; }

    public List<Card> Deck { get; set; } = new List<Card>();

    public Player()
    {
        Deck = new List<Card>();
    }

    public PlayerTurn PlayTurn()
    {
        //TODO - Kata
    }

    public PlayerTurn DrawCard(PlayerTurn previousTurn, CardDeck cardDeck)
    { 
        PlayerTurn turn = new PlayerTurn();
        var drawnCard = cardDeck.Draw(1);
        Deck.AddRange(drawnCard); //what if CardDeck will be empty? call function that will create new shuffled deck
                                  //or shouldnt allow to return null CardDeck? 
        
        if (HasMatch(previousTurn.Card))
        {
        turn = PlayMatchingCard(previousTurn.Card);
        turn.Result = TurnResult.ForceDrawPlay; //enum TurnResult was created by Kata in katbranch
        }
        else
        {
        turn.Result = TurnResult.ForceDraw; //enum TurnResult was created by Kata in katbranch
        turn.Card = previousTurn.Card;
        }
        
        return turn;
    }
    
    public void DisplayTurn()
    {
        //TODO - Kata
    }
    
    public PlayerTurn ProccesAttack()
    {
        //TODO - Kata
    }

    private bool HasMatch(Card card)
    {
        //refactor of Cards.cs and SpecialCards.cs and NumericCard.cs or create function for both of them.
        // here is also matching by color or number(value).
        //possibility control the match just by color input and check matching colorsHas
        return Deck.Any(x => x.Color == card.Color || x.Number == card.Number|| x.Color == EColors.Black);
    }

    private PlayerTurn PlayMatchingCard(Card currentDiscard)
    {
        var turn = new PlayerTurn();
            turn.Result = TurnResult.PlayedCard; //enum by kata
            //refactor of Cards.cs and SpecialCards.cs and NumericCard.cs or create function for both of them.
            var matching = Deck.Where(x =>
                    x.Color == currentDiscard.Color || x.Value == currentDiscard.Value || x.Color == EColors.Black)
                .ToList();

            //We cannot play wild draw four unless there are no other matches.
            if(matching.All(x => x.Value == EEffect.DrawFour)) //card restruct
            {
                turn.Card = matching.First();
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Deck.Remove(matching.First());

                return turn;
            }

            //Otherwise, we play the card that would cause the most damage to the next player.
            if(matching.Any(x=> x.Value == CardValue.DrawTwo))
            {
                turn.Card = matching.First(x => x.Value == CardValue.DrawTwo);
                turn.Result = TurnResult.DrawTwo;
                turn.DeclaredColor = turn.Card.Color;
                Deck.Remove(turn.Card);

                return turn;
            }

            if(matching.Any(x => x.Value == CardValue.Skip))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Skip);
                turn.Result = TurnResult.Skip;
                turn.DeclaredColor = turn.Card.Color;
                Deck.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Reverse))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Reverse);
                turn.Result = TurnResult.Reversed;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            //At this point the player has a choice of sorts
            //Assuming he has a match on color AND a match on value, he can choose which to play
            //For this demo, we'll assume that playing the match with MORE possible plays from his hand is the better option.

            var matchOnColor = matching.Where(x => x.Color == currentDiscard.Color);
            var matchOnValue = matching.Where(x => x.Value == currentDiscard.Value);
            if(matchOnColor.Any() && matchOnValue.Any())
            {
                var correspondingColor = Deck.Where(x => x.Color == matchOnColor.First().Color);
                var correspondingValue = Deck.Where(x => x.Value == matchOnValue.First().Value);
                if(correspondingColor.Count() >= correspondingValue.Count())
                {
                    turn.Card = matchOnColor.First();
                    turn.DeclaredColor = turn.Card.Color;
                    Deck.Remove(matchOnColor.First());

                    return turn;
                }
                else //Match on value
                {
                    turn.Card = matchOnValue.First();
                    turn.DeclaredColor = turn.Card.Color;
                    Deck.Remove(matchOnValue.First());

                    return turn;
                }
                //Figure out which of these is better
            }
            else if(matchOnColor.Any())
            {
                turn.Card = matchOnColor.First();
                turn.DeclaredColor = turn.Card.Color;
                Deck.Remove(matchOnColor.First());

                return turn;
            }
            else if(matchOnValue.Any())
            {
                turn.Card = matchOnValue.First();
                turn.DeclaredColor = turn.Card.Color;
                Deck.Remove(matchOnValue.First());

                return turn;
            }

            //Play regular wilds last.  If a wild becomes our last card, we win on the next turn!
            if (matching.Any(x => x.Value == CardValue.Wild)) //card restruct
            {
                turn.Card = matching.First(x => x.Value == CardValue.Wild);
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Deck.Remove(turn.Card);

                return turn;
            }

            return turn;
    }
            
      private PlayerTurn PlayMatchingCard(CardColor color)
        {
            var turn = new PlayerTurn();
            turn.Result = TurnResult.PlayedCard;
            var matching = Deck.Where(x => x.Color == color || x.Color == CardColor.Wild).ToList();

            //We cannot play wild draw four unless there are no other matches.
            if (matching.All(x => x.Value == CardValue.DrawFour))
            {
                turn.Card = matching.First();
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Deck.Remove(matching.First());

                return turn;
            }

            //Otherwise, we play the card that would cause the most damage to the next player.
            if (matching.Any(x => x.Value == CardValue.DrawTwo))
            {
                turn.Card = matching.First(x => x.Value == CardValue.DrawTwo);
                turn.Result = TurnResult.DrawTwo;
                turn.DeclaredColor = turn.Card.Color;
                Deck.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Skip))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Skip);
                turn.Result = TurnResult.Skip;
                turn.DeclaredColor = turn.Card.Color;
                Deck.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Reverse))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Reverse);
                turn.Result = TurnResult.Reversed;
                turn.DeclaredColor = turn.Card.Color;
                Deck.Remove(turn.Card);

                return turn;
            }

            var matchOnColor = matching.Where(x => x.Color == color);
            if (matchOnColor.Any())
            {
                turn.Card = matchOnColor.First();
                turn.DeclaredColor = turn.Card.Color;
                Deck.Remove(matchOnColor.First());

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Wild))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Wild);
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Deck.Remove(turn.Card);

                return turn;
            }

            //This should never happen
            turn.Result = TurnResult.ForceDraw;
            return turn;
        }

        private EColors SelectDominantColor()
        {
            if (!Deck.Any())
            {
                return EColors.Black;
            }
            var colors = Deck.GroupBy(x => x.Color).OrderByDescending(x => x.Count());
            return colors.First().First().Color;
        }
}