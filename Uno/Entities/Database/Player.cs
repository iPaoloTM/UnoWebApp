using System.ComponentModel.DataAnnotations;

namespace Entities.Database;

public class Player : BaseEntity
{
    [MaxLength(128)] public string Nickname { get; set; } = default!;

    public EPlayerType Type { get; set; }

    public Guid Game { get; set; }
}