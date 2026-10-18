using JassApi.Persistence;

namespace JassApi.UnitTests.Persistence;

public partial class JassApiDbContextModelUnitTests
{
    private static JassApiDbContext CreateContext()
    {
        return new JassApiDbContext(JassApiDbContextOptions.Create(
            "Server=(localdb)\\MSSQLLocalDB;Database=ModelOnly;Trusted_Connection=True;TrustServerCertificate=True"));
    }
}