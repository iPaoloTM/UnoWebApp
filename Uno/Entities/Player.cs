// See https://aka.ms/new-console-template for more information

using System.Buffers;
using System.Net.NetworkInformation;

namespace Entities;


public class Player
{
    public string Nickname { get; }
    public int Position { get; set; } = default!;
    public EPlayerType PlayerTipe { get; set; }

    public List<Card> Hand { get; set; } = new List<Card>();

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

    public EPlayerType PlayerType { get; set; }
    public List<Card> HandCards { get; set; } = new List<Card>();
    public PlayerMove PreviousPlayerMove;
    private String? Reaction { get; set; }
    
    
    public Player(string nickname, PlayerMove previousTurn)
    {
        Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname), "Nickname cannot be null.");
        PreviousPlayerMove = previousTurn;
        HandCards = new List<Card>();
    }
    
    public void AddCard(Card card)
    {
        HandCards.Add(card);

    }
    
    public void TakeCard(Card card)
    {
        if (card == null)
        {
            throw new ArgumentNullException(nameof(card), "Card cannot be null");
        }
        
        if (!HandCards.Contains(card))
        {
            throw new ArgumentException("The specified card is not in the player's hand", nameof(card));
        }

        HandCards.Remove(card);
    }
    

    public PlayerMove PlayCard(Card card)
    {
        var playerMove = new PlayerMove(this, EPlayerAction.PlayCard, card);
        return playerMove; 

    }

    public PlayerMove Draw( )
    {
        var playerMove = new PlayerMove(this, EPlayerAction.Draw, null);
        return playerMove;
    }

    public PlayerMove NextPlayer()
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

