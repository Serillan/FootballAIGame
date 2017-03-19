using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootballAIGameWeb.Models
{
    /// <summary>
    /// Represents the recurring tournament. It keeps references to all currently
    /// planned tournaments from this recurrence.
    /// </summary>
    /// <seealso cref="FootballAIGameServer.Models.TournamentBase" />
    public class RecurringTournament : TournamentBase
    {
        /// <summary>
        /// Gets or sets the recurrence interval.
        /// </summary>
        /// <value>
        /// The recurrence interval.
        /// </value>
        [DisplayName("Recurrence Interval (minutes)")]
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