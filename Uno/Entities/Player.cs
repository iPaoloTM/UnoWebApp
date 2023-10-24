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

    public PlayerTurn DrawCard()
    {

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

    private bool hasMatch()
    {
        //TODO - Michal

    }

    private PlayerTurn PlayMatchingCard()
    {
        //TODO - Michal
    }
    
    
}