namespace JassApi.Persistence.Entities;

public sealed class TrumpfrundeEntity
{
    public int CoiffeurSpielrundeId { get; set; }

    public CoiffeurTrumpfTyp CoiffeurTrumpfTyp { get; set; }
    public int Id { get; set; }

    public bool IstKonterMatchTeam1 { get; set; }

    public bool IstKonterMatchTeam2 { get; set; }

    public bool IstMatschTeam1 { get; set; }

    public bool IstMatschTeam2 { get; set; }

    public int PunkteModifikator { get; set; }

    public int? ResultatTeam1 { get; set; }

    public int? ResultatTeam2 { get; set; }
}