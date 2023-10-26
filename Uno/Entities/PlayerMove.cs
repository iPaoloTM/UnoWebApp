namespace Entities;

public class PlayerMove
{
    public Player Player { get; set; }
    public Card? PlayedCard { get; set; }
    public EPlayerAction PlayerAction{ get; set; }

    

    public PlayerMove(Player player, EPlayerAction playerAction, Card? playedCard)
    {
        this.PlayerAction = playerAction;
        this.PlayedCard = playedCard;
        this.Player = player;
    }

}