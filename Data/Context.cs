using LogicLayer.Modules.LoggingModule.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework;

public class Context(DbContextOptions<Context> options) : DbContext(options)
{
    public DbSet<LogMessage> LogMessages { get; set; }
}