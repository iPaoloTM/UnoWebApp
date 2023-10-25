namespace Entities;

public class NumericCard : Card
{
    public ENumbers Number { get; set; }
    
    public override string ToString()
    {
        return "{\"CardType\":\"Numeric\", \"Color\": \""+this.Color+"\", \"Number\": \""+this.Number+"\"}";
    }
    
}