using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BattleShip2077
{
    public class Result
    {
        public int ID { get; set; }
        public string? PLAYER_WIN { get; set; }
        public string? BOT_WIN { get; set; }
        public string? LOGIN { get; set; }
        public DateTime DATE_TIME { get; set; }

    }
    public class Log
    {
        public int ID { get; set; }
        public string? login { get; set; }
        public string? password { get; set; }

    }
    public class Protocol
    {
        public int ID { get; set; }
        public string? login { get; set; }
        public string? player_shoot { get; set; }
        public string? bot_shoot { get; set; }
        public string? start_game { get; set; }
        public string? end_game { get; set; }
        public string? victory { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Result> Results { get; set; } = null!;
        public DbSet<Log> Logs { get; set; } = null!;
        public DbSet<Protocol> Protocols { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
           : base(options)
        {
            Database.EnsureCreated();
        }
    }
}