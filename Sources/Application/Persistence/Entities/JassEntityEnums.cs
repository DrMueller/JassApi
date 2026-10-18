namespace JassApi.Persistence.Entities;

public enum JassTeamTyp
{
    Team1,
    Team2
}

public enum JassTeamSpielerPosition
{
    Spieler1 = 1,
    Spieler2 = 2
}

public enum CoiffeurTrumpfTyp
{
    Herz = 0,
    Ecken = 1,
    Kreuz = 2,
    Schaufeln = 3,
    Gschobna = 4,
    Differenzler = 5,
    Oben = 6,
    Unten = 7,
    Slalom = 8,
    Misere = 9,
    Gustav = 10,
    DrueliDrue = 11,
    Tutti = 12,
    Wahl = 13
}

public enum CoiffeurSpielrundeTyp
{
    WithGschobna,
    WithDifferenzler,
    OhneBeides
}