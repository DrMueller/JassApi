namespace JassApi.Persistence.Entities;

public sealed class JassTeamSpielerEntity
{
    public int Id { get; set; }

    public bool IstStartSpieler { get; set; }

    public JassTeamEntity JassTeam { get; set; } = null!;

    public int JassTeamId { get; set; }

    public JassTeamSpielerPosition Position { get; set; }

    public SpielerEntity Spieler { get; set; } = null!;

    public int SpielerId { get; set; }
}