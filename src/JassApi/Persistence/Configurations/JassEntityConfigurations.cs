using JassApi.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JassApi.Persistence.Configurations;

internal sealed class SpielerEntityConfiguration : IEntityTypeConfiguration<SpielerEntity>
{
    public void Configure(EntityTypeBuilder<SpielerEntity> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
        builder.Property(entity => entity.Name).IsRequired();
        builder.HasMany(entity => entity.TeamPlayers)
            .WithOne(teamPlayer => teamPlayer.Spieler)
            .HasForeignKey(teamPlayer => teamPlayer.SpielerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.ToTable("SpielerTable");
    }
}

internal sealed class JassTeamSpielerEntityConfiguration : IEntityTypeConfiguration<JassTeamSpielerEntity>
{
    public void Configure(EntityTypeBuilder<JassTeamSpielerEntity> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
        builder.Property(entity => entity.JassTeamId).IsRequired();
        builder.Property(entity => entity.SpielerId).IsRequired();
        builder.Property(entity => entity.Position).IsRequired();
        builder.Property(entity => entity.IstStartSpieler).IsRequired();
        builder.HasOne(entity => entity.Spieler)
            .WithMany(player => player.TeamPlayers)
            .HasForeignKey(entity => entity.SpielerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(entity => entity.JassTeam)
            .WithMany(team => team.JassTeamSpieler)
            .HasForeignKey(entity => entity.JassTeamId);
        builder.ToTable("JassTeamSpielerTable");
    }
}

internal sealed class JassTeamEntityConfiguration : IEntityTypeConfiguration<JassTeamEntity>
{
    public void Configure(EntityTypeBuilder<JassTeamEntity> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
        builder.Property(entity => entity.JassTeamTyp).IsRequired();
        builder.Property(entity => entity.CoiffeurSpielrundeId).IsRequired();
        builder.ToTable("JassTeamTable");
    }
}

internal sealed class TrumpfrundeEntityConfiguration : IEntityTypeConfiguration<TrumpfrundeEntity>
{
    public void Configure(EntityTypeBuilder<TrumpfrundeEntity> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
        builder.Property(entity => entity.CoiffeurTrumpfTyp).IsRequired();
        builder.Property(entity => entity.PunkteModifikator).IsRequired();
        builder.Property(entity => entity.CoiffeurSpielrundeId).IsRequired();
        builder.Property(entity => entity.ResultatTeam1).IsRequired(false);
        builder.Property(entity => entity.ResultatTeam2).IsRequired(false);
        builder.ToTable("TrumpfrundeTable");
    }
}

internal sealed class CoiffeurSpielrundeEntityConfiguration : IEntityTypeConfiguration<CoiffeurSpielrundeEntity>
{
    public void Configure(EntityTypeBuilder<CoiffeurSpielrundeEntity> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
        builder.Property(entity => entity.GestartetAm).IsRequired();
        builder.Property(entity => entity.Punktewert).IsRequired();
        builder.Property(entity => entity.CoiffeurSpielrundeTyp).IsRequired();
        builder.Property(entity => entity.DoIncludeRaucherpausen).IsRequired();
        builder.Property(entity => entity.DoIncludeShots).IsRequired();
        builder.Property(entity => entity.IsJassTrainingslager).IsRequired();
        builder.HasMany(entity => entity.Trumpfrunden)
            .WithOne()
            .HasForeignKey(trumpRound => trumpRound.CoiffeurSpielrundeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(entity => entity.JassTeams)
            .WithOne()
            .HasForeignKey(team => team.CoiffeurSpielrundeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.ToTable("CoiffeurSpielrundeTable");
    }
}