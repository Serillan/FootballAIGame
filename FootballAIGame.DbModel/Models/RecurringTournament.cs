using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FootballAIGame.DbModel.Models
{
    /// <summary>
    /// Represents the recurring tournament. It keeps references to all currently
    /// planned tournaments from this recurrence.
    /// </summary>
    /// <seealso cref="FootballAIGame.DbModel.Models.TournamentBase" />
    public class RecurringTournament : TournamentBase
    {
        /// <summary>
        /// Gets or sets the recurrence interval.
        /// </summary>
        /// <value>
        /// The recurrence interval.
        /// </value>
        [DisplayName("Recurrence Interval (minutes)")]
        [Range(1, 525600)]
        public int RecurrenceInterval { get; set; }

        /// <summary>
        /// Gets or sets the number of present tournaments.
        /// It's the number of running and unstarted tournaments that
        /// are present at any time. If any tournament is closed or finished
        /// then another tournament from this recurrence is created.
        /// </summary>
        /// <value>
        /// The number of present tournaments.
        /// </value>
        [DisplayName("Number of Present Tournaments")]
        [Range(1, 100)]
        public int NumberOfPresentTournaments { get; set; }

        /// <summary>
        /// Gets or sets the tournaments belonging to this recurrence.
        /// </summary>
        /// <value>
        /// The tournaments.
        /// </value>
        public ICollection<Tournament> Tournaments { get; set; }
    }
}