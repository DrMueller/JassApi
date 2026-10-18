using JassApi.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace JassApi.Persistence;

public static class JassApiDbContextOptions
{
    public static DbContextOptions<JassApiDbContext> Create(string connectionString)
    {
        var modelBuilder = new ModelBuilder(SqlServerConventionSetBuilder.Build());
        modelBuilder.ApplyConfiguration(new SpielerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JassTeamSpielerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JassTeamEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TrumpfrundeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CoiffeurSpielrundeEntityConfiguration());

        return new DbContextOptionsBuilder<JassApiDbContext>()
            .UseSqlServer(connectionString)
            .UseModel(modelBuilder.FinalizeModel())
            .Options;
    }
}