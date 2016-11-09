using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using NBAStatistics.Data.Repositories.Contracts;
using NBAStatistics.DataImporters.Contracts;
using NBAStatistics.Models;

using PlayerMongo = NBAStatistics.Data.FillMongoDB.Models.Player;

namespace NBAStatistics.DataImporters
{
    public class PlayersImporter : IDataImporter
    {
        private readonly IRepository<PlayerMongo> sourceRepository;
        private readonly IEfRepository<Player> playersRepository;
        private readonly IEfRepository<Team> teamsRepository;
        private readonly IUnitOfWork unitOfWork;

        public PlayersImporter(
            IRepository<PlayerMongo> sourceRepository,
            IEfRepository<Player> playersRepository,
            IEfRepository<Team> teamsRepository,
            IUnitOfWork unitOfWork)
        {
            this.sourceRepository = sourceRepository;
            this.teamsRepository = teamsRepository;
            this.playersRepository = playersRepository;
        }

        public async Task Import()
        {
            await Task.Run(async () =>
            {
                var mongoPlayers = this.sourceRepository.GetAll().ToList();

                // Load all players from the database into the db Context 
                this.playersRepository.Context.Set<Player>().Load();

                // Loads all teams from database into db Context
                this.teamsRepository.Context.Set<Team>().Load();

                foreach (var player in mongoPlayers)
                {
                    // throws an exception if there is more than 1 element in the sequence
                    var playerInDb = this.playersRepository.Context
                        .Set<Player>()
                        .Local
                        .SingleOrDefault(p => p.AdditionalInfo.PlayerId == player.PlayerId); // runs in memory

                    if (playerInDb == null)
                    {
                        this.playersRepository.Add(new Player
                        {
                            FirstName = player.FirstName,
                            LastName = player.LastName,
                            AdditionalInfo = new PlayerInfo
                            {
                                PlayerId = player.PlayerId,
                                Birthday = player.BirthDate,
                                Height = string.IsNullOrEmpty(player.Height) ? null : ConvertHeightFromFeetsInchesToCentimeters(player.Height),
                                Weight = string.IsNullOrEmpty(player.Weight) ? null : ConvertPoundsToKilogram(player.Weight)
                            },
                            School = string.IsNullOrEmpty(player.School) ? null : new School
                            {
                                Name = player.School
                            },
                            Country = string.IsNullOrEmpty(player.Country) ?
                                new Country { Name = "NoName" } :
                                    string.IsNullOrEmpty(player.Country.Trim()) ?
                                        new Country { Name = "NoName" } :
                                        new Country { Name = player.Country },
                            Position = player.Position,
                            RosterStatus = player.RosterStatus,
                            TeamId = this.teamsRepository.Context
                                .Set<Team>()
                                .Local
                                .Single(t => t.TeamId == player.TeamId)
                                .Id
                        });
                    }
                }

                await this.unitOfWork.CommitAsync();
            });
        }

        private double? ConvertHeightFromFeetsInchesToCentimeters(string sHeight)
        {
            if (string.IsNullOrEmpty(sHeight))
            {
                return null;
            }

            var feets = int.Parse(sHeight.Split(new char[] { '-' })[0]);
            var inches = int.Parse(sHeight.Split(new char[] { '-' })[1]);

            var totalInches = (feets * 12) + inches;
            var centimeters = totalInches * 2.54;

            return Math.Round(centimeters, 1);
        }

        public static double? ConvertPoundsToKilogram(string sPounds)
        {
            if (string.IsNullOrEmpty(sPounds))
            {
                return null;
            }

            return Math.Round(int.Parse(sPounds) / 2.20462262);
        }
    }
}
