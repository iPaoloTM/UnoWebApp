// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using Entities;

public class Player
{
    public string? Nickname { get; set; }
    public int Position { get; set; } = default!;
    public EPlayerType PlayerTipe { get; set; }

    public List<Card> Deck { get; set; } = new List<Card>();

    public Player()
    {
        Deck = new List<Card>();
    }

    public PlayerTurn PlayTurn(PlayerTurn previousTurn, CardDeck pileOfCards)
    {
        //TODO - Kata
        PlayerTurn turn = new PlayerTurn();
        if (previousTurn.Result == ETurnResult.Skip || previousTurn.Result == ETurnResult.DrawTwoCards 
                                                    || previousTurn.Result == ETurnResult.WildDrawFourCards)
        {
            return ProcessAttack(previousTurn.Card, pileOfCards);
        }
        else if ((previousTurn.Result == ETurnResult.WildCard || previousTurn.Result == ETurnResult.IsAttacked 
                                                              || previousTurn.Result == ETurnResult.ForcedDraw) && HasMatch(previousTurn.DeclaredColor)) //will be implemented by michal i hope
        {
            turn = PlayMatchingCard(previousTurn.DeclaredColor);
        }
        else if (HasMatch(previousTurn.Card)) //will be implemented by michal i hope
        {
            turn = PlayMatchingCard(previousTurn.Card);
        }
        else 
        {
            turn = DrawCard(previousTurn, pileOfCards);
        }

        DisplayTurn(turn);
        return turn;
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
    
    public void DisplayTurn(PlayerTurn turn)
    {
        //TODO - Kata ---- WHAT IS IN CONSOLE NEEDS TO BE MODIFIED
        if (turn.Result == ETurnResult.ForcedDrawPlay)
        {
            Console.WriteLine("Player is forced to draw a card since he can't put anything");
        }
        if(turn.Result == ETurnResult.ForcedDrawPlay)
        {
            Console.WriteLine("Player is forced to draw,  but can play the drawn card!");
        }
        if (turn.Result == ETurnResult.PlayedCard || turn.Result == ETurnResult.Skip || turn.Result == ETurnResult.DrawTwoCards 
                                                  || turn.Result == ETurnResult.WildCard || turn.Result == ETurnResult.WildDrawFourCards 
                                                  || turn.Result == ETurnResult.Reversed || turn.Result == ETurnResult.ForcedDrawPlay)
        {
            Console.WriteLine("Player plays a card.");
            //!!!!!!!!!!!!!!!!
            if (turn.Card is SpecialCard specialCard)
            {
                if (specialCard.Effect == EEffect.Wild) // here i'd change the logic of declaring a card but i didn't do it because i don't want to spoil anything :(
                {
                    Console.WriteLine("Player declares new color.");
                }
            }
            else
            {
                throw new InvalidOperationException("Card is of an unexpected type"); //we can also handle it somehow else that is just an idea
            }
            if(turn.Result == ETurnResult.Reversed)
            {
                Console.WriteLine("Turn order reversed!");
            }
        }

        if (Deck.Count == 1)
        {
            //TODO - think about shouting UNO logic
            Console.WriteLine("Player shouts UNO but WE NEED TO THINK ABOUT IT!!!!!!!!!!!");
        }
    }
    
    public PlayerTurn ProcessAttack( Card currentCard, CardDeck pileOfCards)
    {
        //TODO - Kata
        PlayerTurn turn = new PlayerTurn();
        turn.Result = ETurnResult.IsAttacked;
        turn.Card = currentCard;
        turn.DeclaredColor = currentCard.Color;
        //!!!!!!!!!!!!!!!
        if (currentCard is  SpecialCard specialCard)
        {
            if(specialCard.Effect == EEffect.Skip)
            {
                Console.WriteLine("Player  was skipped!");
                return turn;
            }
            else if(specialCard.Effect == EEffect.DrawTwo)
            {
                Console.WriteLine("Player  must draw two cards!");
                Deck.AddRange(pileOfCards.Draw(2));
            }
            else if(specialCard.Effect == EEffect.DrawFour)
            {
                Console.WriteLine("Player  must draw four cards!");
                Deck.AddRange(pileOfCards.Draw(4));
            }
        }
        else
        {
            throw new InvalidOperationException("Card is of an unexpected type"); //we can also handle it somehow else that is just an idea
        }
        

        return turn;
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
            
      private PlayerTurn PlayMatchingCard(EColors color)
        {
            var turn = new PlayerTurn();
            turn.Result = ETurnResult.PlayedCard;
            var matching = Deck.Where(x => x.Color == color || x.Color == EColors.Black).ToList();

            //We cannot play wild draw four unless there are no other matches.
            if (matching.All(x => x.Value == EEffect.DrawFour))
            {
                turn.Card = matching.First();
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = ETurnResult.WildCard;
                Deck.Remove(matching.First());

                return turn;
            }

            //Otherwise, we play the card that would cause the most damage to the next player.
            if (matching.Any(x => x.Value == EEffect.DrawTwo))
            {
                turn.Card = matching.First(x => x.Value == EEffect.DrawTwo);
                turn.Result = ETurnResult.DrawTwo;
                turn.DeclaredColor = turn.Card.Color;
                Deck.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == EEffect.Skip))
            {
                turn.Card = matching.First(x => x.Value == EEffect.Skip);
                turn.Result = ETurnResult.Skip;
                turn.DeclaredColor = turn.Card.Color;
                Deck.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == EEffect.Reverse))
            {
                turn.Card = matching.First(x => x.Value == EEffect.Reverse);
                turn.Result = ETurnResult.Reversed;
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

            if (matching.Any(x => x.Value == EEffect.Wild))
            {
                turn.Card = matching.First(x => x.Value == EEffect.Wild);
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = ETurnResult.WildCard;
                Deck.Remove(turn.Card);

                return turn;
            }

            //This should never happen
            turn.Result = ETurnResult.ForcedDraw;
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