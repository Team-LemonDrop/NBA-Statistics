using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using NBAStatistics.Models;

namespace NBAStatistics.Data.Contracts.SQLServer
{
    public interface INBAStatisticsDbContext
    {
        IDbSet<Arena> Arenas { get; set; }

        IDbSet<City> Cities { get; set; }

        IDbSet<Conference> Conferences { get; set; }

        IDbSet<Country> Countries { get; set; }

        IDbSet<HeadCoach> HeadCoaches { get; set; }

        IDbSet<Player> Players { get; set; }

        IDbSet<School> Schools { get; set; }

        IDbSet<StandingsByDay> StandingsByDays { get; set; }

        IDbSet<Team> Teams { get; set; }

        IDbSet<T> Set<T>() where T : class;

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        int SaveChanges();
    }
}

