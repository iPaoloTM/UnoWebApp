namespace Entities;

public abstract class Deck
{
    

    public List<Card> Cards { get; set; } = new();
    private Random Rnd { get; set; } = new Random();

    //Changed Shuffle to be shorter and more readable
    public void Shuffle()
    {
        int n = Cards.Count;
        //i will count down from the biggest index to 0
        for (int i = n - 1; i > 0; i--)
        {
            //Get a random number between 0 and the value of i
            int j = Rnd.Next(i + 1);
            //Swap the rightmost element with the random element
            (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
        }
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