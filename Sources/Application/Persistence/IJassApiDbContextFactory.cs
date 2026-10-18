namespace JassApi.Persistence;

public interface IJassApiDbContextFactory
{
    int CreateCount { get; }
    JassApiDbContext Create();
}