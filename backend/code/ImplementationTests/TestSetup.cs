using Database;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ImplementationTests;

[SetUpFixture]
public static class TestSetup
{
    private static AppDbContext _context;

    public static AppDbContext Context
    {
        get => _context;
    }

    [OneTimeSetUp]
    public static void GlobalSetup()
    {
        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDb")
        );

        services.AddScoped<DbContext, AppDbContext>();

        var provider = services.BuildServiceProvider();
        _context = provider.GetRequiredService<AppDbContext>();
    }

    [OneTimeTearDown]
    public static void GlobalTearDown()
    {
        _context?.Dispose();
        _context = null;
    }
}