namespace Entities;

public class PlayerTurn
{
    public Card? Card { get; set; }
    public EColors DeclaredColor { get; set; }
    public ETurnResult Result { get; set; }

}