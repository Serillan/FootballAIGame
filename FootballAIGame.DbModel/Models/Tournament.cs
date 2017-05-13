using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FootballAIGame.DbModel.Models
{
    /// <summary>
    /// Represents the tournament.
    /// </summary>
    /// <seealso cref="FootballAIGame.DbModel.Models.TournamentBase" />
    public class Tournament : TournamentBase
    {
        /// <summary>
        /// Gets or sets the state of the tournament.
        /// </summary>
        /// <value>
        /// The state of the tournament.
        /// </value>
        [Required]
        public TournamentState TournamentState { get; set; }

        /// <summary>
        /// Gets or sets the tournament to which this tournament belongs.
        /// If the tournament is not part of a tournament it is equal to null.
        /// </summary>
        public RecurringTournament RecurringTournament { get; set; }

        /// <summary>
        /// Gets or sets the collection of players that are participating in the tournament.
        /// </summary>
        /// <value>
        /// The players participating.
        /// </value>
        public ICollection<TournamentPlayer> Players { get; set; }

        /// <summary>
        /// Gets or sets the tournament's matches.
        /// </summary>
        /// <value>
        /// The tournament's matches.
        /// </value>
        public ICollection<Match> Matches { get; set; }
    }
}