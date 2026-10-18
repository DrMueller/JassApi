namespace JassApi.Persistence.Entities;

public sealed class CoiffeurSpielrundeEntity
{
    public CoiffeurSpielrundeTyp CoiffeurSpielrundeTyp { get; set; }

    public bool DoIncludeRaucherpausen { get; set; }

    public bool DoIncludeShots { get; set; }

    public DateTime GestartetAm { get; set; }
    public int Id { get; set; }

    public bool IsJassTrainingslager { get; set; }

    public ICollection<JassTeamEntity> JassTeams { get; set; } = [];

    public int Punktewert { get; set; }

    public ICollection<TrumpfrundeEntity> Trumpfrunden { get; set; } = [];
}