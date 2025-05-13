using Database;
using Entities;

namespace Tools;

public static class Logger
{
    private static AppDbContext _context;

    public static void Initialize(AppDbContext context)
    {
        _context = context;
    }

    public static void Log(int greenhouseId, string message)
    {
        _context.Logs.AddAsync(new Log
        {
            GreenhouseId = greenhouseId,
            Timestamp = DateTime.UtcNow,
            Message = message
        });

        _context.SaveChangesAsync();
    }
}