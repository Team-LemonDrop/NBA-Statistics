using System.Collections.Generic;

namespace NBAStatistics.Models
{
    public class Conference
    {
        private ICollection<StandingsByDay> standingsByDay;

        public Conference()
        {
            this.standingsByDay = new HashSet<StandingsByDay>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<StandingsByDay> StandingsByDay
        {
            get { return this.standingsByDay; }
            set { this.standingsByDay = value; }
        }
    }
}
