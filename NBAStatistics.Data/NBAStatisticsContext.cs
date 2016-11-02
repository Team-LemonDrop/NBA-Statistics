using System.Data.Entity;

using NBAStatistics.Data.Migrations;
using NBAStatistics.Models;

namespace NBAStatistics.Data
{
    public class NBAStatisticsContext : DbContext
    {
        public NBAStatisticsContext()
            : base("name=NBAStatisticsConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<NBAStatisticsContext, Configuration>());
        }

        public IDbSet<Arena> Arenas { get; set; }

        public IDbSet<City> Cities { get; set; }

        public IDbSet<Conference> Conferences { get; set; }

        public IDbSet<Country> Countries { get; set; }

        public IDbSet<HeadCoach> HeadCoaches { get; set; }

        public IDbSet<Player> Players { get; set; }

        public IDbSet<School> Schools { get; set; }

        public IDbSet<StandingsByDay> StandingsByDays { get; set; }

        public IDbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
