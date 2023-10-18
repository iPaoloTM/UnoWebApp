namespace Domain;

public class Player
{
    public string NickName { get; set; } = default!;
    public EPlayerType PlayerType { get; set; }
    
    public List<GameCard> PlayerHand { get; set; } = new List<GameCard>();
}
