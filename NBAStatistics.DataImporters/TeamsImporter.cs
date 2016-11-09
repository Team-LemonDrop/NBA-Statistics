using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using NBAStatistics.Data.Repositories.Contracts;
using NBAStatistics.DataImporters.Contracts;
using NBAStatistics.Models;

using SeasonMongo = NBAStatistics.Data.FillMongoDB.Models.Season;

namespace NBAStatistics.DataImporters
{
    public class TeamsImporter : IDataImporter
    {
        private readonly IRepository<SeasonMongo> sourceRepository;
        private readonly IEfRepository<Team> teamsRepository;
        private readonly IUnitOfWork unitOfWork;

        public TeamsImporter(
            IRepository<SeasonMongo> sourceRepository,
            IEfRepository<Team> teamsRepository,
            IUnitOfWork unitOfWork)
        {
            this.sourceRepository = sourceRepository;
            this.teamsRepository = teamsRepository;
        }

        public async Task Import()
        {
            await Task.Run(async () =>
            {
                using (this.unitOfWork)
                {
                    var seasons = this.sourceRepository.GetAll().ToList();

                    // Loads all entities into dbContext.
                    this.teamsRepository.Context.Set<Team>().Load();

                    var nullTeamInDb = this.teamsRepository.Context.Set<Team>()
                        .Local
                        .SingleOrDefault(t => t.TeamId == 0);

                    if (nullTeamInDb == null)
                    {
                        // add fake team for players without team
                        this.teamsRepository.Add(new Team
                        {
                            TeamId = 0,
                            Name = "NoName",
                            Abbreviation = "",
                            Founded = 0,
                            City = new City
                            {
                                Name = "NoName",
                                Country = new Country
                                {
                                    Name = "NoName"
                                }
                            },
                            Arena = new Arena
                            {
                                Name = "NoName"
                            },
                            HeadCoach = new HeadCoach
                            {
                                Name = "NoName"
                            }
                        });
                    }

                    foreach (var season in seasons)
                    {
                        foreach (var team in season.Teams)
                        {
                            var teamInDb = this.teamsRepository.Context
                                .Set<Team>()
                                .Local
                                .SingleOrDefault(t => t.TeamId == team.TeamId); // runs in memory

                            if (teamInDb == null)
                            {
                                this.teamsRepository
                                .Add(new Team
                                {
                                    TeamId = team.TeamId,
                                    Name = team.Name,
                                    Abbreviation = team.Abbreviation,
                                    Founded = team.Founded,
                                    City = new City
                                    {
                                        Name = team.City,
                                        Country = new Country
                                        {
                                            Name = team.Country
                                        }
                                    },
                                    Arena = new Arena
                                    {
                                        Name = team.Arena
                                    },
                                    HeadCoach = new HeadCoach
                                    {
                                        Name = team.HeadCoach
                                    }
                                });
                            }
                        }
                    }

                    await this.unitOfWork.CommitAsync();
                }
            });
        }
    }
}
