namespace Entities;

public class SpecialCard : Card
{
    public EEffect Effect { get; set; }
    
    public override string ToString()
    {
        return "{\"CardType\":\"Special\", \"Color\": \""+this.Color+"\", \"Effect\": \""+this.Effect+"\"}";
    }
}