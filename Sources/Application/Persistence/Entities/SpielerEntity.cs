namespace JassApi.Persistence.Entities;

public sealed class SpielerEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<JassTeamSpielerEntity> TeamPlayers { get; set; } = [];
}