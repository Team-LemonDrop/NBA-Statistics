using System.Collections.Generic;

namespace NBAStatistics.Reports.Pocos
{
    public class TeamDailyStandingsPoco
    {
        public string TeamName { get; set; }

        public IEnumerable<GamePoco> Games { get; set; }
    }
}
