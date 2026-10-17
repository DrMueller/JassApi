namespace JassApi.Persistence.Entities;

public sealed class JassTeamEntity
{
    public int CoiffeurSpielrundeId { get; set; }
    public int Id { get; set; }

    public ICollection<JassTeamSpielerEntity> JassTeamSpieler { get; set; } = [];

    public JassTeamTyp JassTeamTyp { get; set; }
}