using Microsoft.EntityFrameworkCore;

namespace JassApi.Persistence;

public sealed class JassApiDbContextFactory(DbContextOptions<JassApiDbContext> options) : IJassApiDbContextFactory
{
    public JassApiDbContext Create()
    {
        CreateCount++;
        return new JassApiDbContext(options);
    }

    public int CreateCount { get; private set; }
}