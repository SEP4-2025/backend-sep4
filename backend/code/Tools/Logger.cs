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

    public static void Log(string message)
    {
        _context.Logs.AddAsync(new Log
        {
            Timestamp = DateTime.Now,
            Message = message
        });
    }
}