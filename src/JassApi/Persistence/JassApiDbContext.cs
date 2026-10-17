using JassApi.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace JassApi.Persistence;

public sealed class JassApiDbContext(DbContextOptions<JassApiDbContext> options) : DbContext(options)
{
    public DbSet<SpielerEntity> Players => Set<SpielerEntity>();

    public DbSet<CoiffeurSpielrundeEntity> Rounds => Set<CoiffeurSpielrundeEntity>();

    public DbSet<JassTeamSpielerEntity> TeamPlayers => Set<JassTeamSpielerEntity>();

    public DbSet<JassTeamEntity> Teams => Set<JassTeamEntity>();

    public DbSet<TrumpfrundeEntity> TrumpRounds => Set<TrumpfrundeEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JassApiDbContext).Assembly);
    }
}