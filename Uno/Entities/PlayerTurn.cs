namespace Entities;

public class PlayerTurn
{
    public Card? Card { get; set; }
    public EColors DeclaredColor { get; set; } 
    public TurnResult Result { get; set; }

}