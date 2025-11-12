using LogicLayer.Modules.LoggingModule.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        
        public DbSet<Feedback> FeedbackConditions { get; set; }
        public DbSet<Location> ScreenLocations { get; set; }
    }
}