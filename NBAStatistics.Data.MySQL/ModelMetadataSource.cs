using System.Collections.Generic;

using Telerik.OpenAccess.Metadata.Fluent;
using Telerik.OpenAccess.Metadata;

using NBAStatistics.Data.MySQL.Models;

namespace NBAStatistics.Data.MySQL
{
    public class ModelMetadataSource : FluentMetadataSource
    {
        protected override IList<MappingConfiguration> PrepareMapping()
        {
            List<MappingConfiguration> modelsConfigurations = new List<MappingConfiguration>();            MappingConfiguration playerPointsPerGameConfig = this.PreparePlayerPointsPerGameConfiguration();
            modelsConfigurations.Add(playerPointsPerGameConfig);

            return modelsConfigurations;
        }

        protected override void SetContainerSettings(MetadataContainer container)
        {
            container.Name = "Model";
            container.DefaultNamespace = "NBAStatistics.Data.MySQL.Models";
            container.NameGenerator.SourceStrategy = NamingSourceStrategy.Property;
            container.NameGenerator.RemoveCamelCase = false;
        }

        private MappingConfiguration PreparePlayerPointsPerGameConfiguration()
        {
            MappingConfiguration<PlayerPointsPerGame> configuration = new MappingConfiguration<PlayerPointsPerGame>();            configuration.MapType(x => new
            {
                Id = x.Id,
                PlayerId = x.PlayerId,
                PlayerName = x.PlayerName,
                TeamName = x.TeamName,
                PointsPerGame = x.PointsPerGame
            })
            .ToTable("PlayerPointsPerGame");

            configuration.HasProperty(x => x.Id)
                .IsIdentity(KeyGenerator.Autoinc);

            return configuration;
        }
    }
}
