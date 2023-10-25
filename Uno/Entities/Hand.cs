namespace Entities;

public abstract class Hand
{
    

    protected List<Card> Cards { get; set; }
    private Random rnd { get; set; } = new Random();
    
    public Hand()
    {
        this.Cards = new List<Card>();
    }

    public void Shuffle()
    {
        List<Card?> randomDeck = new List<Card?>(); 

        // Initialize randomDeck with nulls.
        for (int k = 0; k < GameState.NumberOfCards; k++)
        {
            randomDeck.Add(null);
        }
        //for the first half of the deck, we randomly map cards from the deck to the new randomDeck,
        //in order to be sure that the conflicts are still not so probable.
        int i = 0;
        while (i < GameState.NumberOfCards / 2)
        {
            var randomPositionInDeck = rnd.Next(GameState.NumberOfCards);
            //we make sure to put the card in an empty spot
            while (randomDeck[randomPositionInDeck] != null)
            {
                randomPositionInDeck = rnd.Next(GameState.NumberOfCards);
            }

            randomDeck[randomPositionInDeck] = Cards?[randomPositionInDeck];
            if (Cards != null) Cards.RemoveAt(randomPositionInDeck);
            i++;
        }
        
        //Now we map the remaining cards from the deck into the empty spots of the random deck
        //to avoid a lot of new random assignment, since checking for conflictsd becomes harder
        //and harder the more card we put (the less empty spots there are)

        if (Cards != null)
        {
            int j = 0;
            while (j < this.Cards.Count)
            {
                
                int k = 0;
                while (randomDeck[k] != null)
                {
                    k++;
                }

                randomDeck[k] = Cards[j];
                
                j++;
            }
        }

        if (randomDeck.Count == GameState.NumberOfCards)
        {
            this.Cards = randomDeck;
        }
        else throw new Exception("Something went wrong when shuffling the deck");


    }
    
    public Card First()
    {
        return this.Cards.First();
    }

    public void Insert(int index, Card card)
    {
        this.Cards.Insert(index, card);
    }

    public void Add(Card card)
    {
        this.Cards.Add(card);
    }
    
}