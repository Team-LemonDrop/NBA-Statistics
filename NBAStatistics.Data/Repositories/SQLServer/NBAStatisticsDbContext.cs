using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using NBAStatistics.Data.Contracts.SQLServer;
using NBAStatistics.Data.Migrations;
using NBAStatistics.Models;

namespace NBAStatistics.Data.Repositories.SQLServer
{
    public class NBAStatisticsDbContext : DbContext, INBAStatisticsDbContext
    {
        public NBAStatisticsDbContext()
            : base("name=NBAStatisticsConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<NBAStatisticsDbContext, Configuration>());
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

        public new IDbSet<T> Set<T>()
            where T : class
        {
            return base.Set<T>();
        }

        public new DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity)
            where TEntity : class
        {
            return base.Entry<TEntity>(entity);
        }

        public new int SaveChanges()
        {
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
