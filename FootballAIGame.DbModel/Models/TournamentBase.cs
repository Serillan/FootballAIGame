using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FootballAIGame.DbModel.Models
{
    /// <summary>
    /// Represents the tournaments' information that is shared between
    /// <see cref="Tournament"/> and <see cref="RecurringTournament"/>.
    /// </summary>
    public abstract class TournamentBase
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the tournament.
        /// </summary>
        /// <value>
        /// The tournament name.
        /// </value>
        [Required, DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the start time of the tournament.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of players that can participate in the tournament.
        /// </summary>
        /// <value>
        /// The maximum number of players.
        /// </value>
        [DisplayName("Maximum Number of Players")]
        [Range(0, 100000)]
        public int MaximumNumberOfPlayers { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of players that have to participate in the tournament for
        /// the tournament to be able to start.
        /// </summary>
        /// <value>
        /// The minimum number of players.
        /// </value>
        [DisplayName("Minimum Number of Players")]
        [Range(0, 100000)]
        public int MinimumNumberOfPlayers { get; set; }
    }
}