using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootballAIGameServer.Models
{
    public class Tournament
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
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the state of the tournament.
        /// </summary>
        /// <value>
        /// The state of the tournament.
        /// </value>
        [Required]
        public TournamentState TournamentState { get; set; }

        /// <summary>
        /// Gets or sets the start time of the tournament.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of players that can participate in the tournament.
        /// </summary>
        /// <value>
        /// The maximum number of players.
        /// </value>
        public int MaximumNumberOfPlayers { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of players that have to participate in the tournament for
        /// the tournament to be able to start.
        /// </summary>
        /// <value>
        /// The minimum number of players.
        /// </value>
        public int MinimumNumberOfPlayers { get; set; }

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

    public enum TournamentState
    {
        /// <summary>
        /// Tournament has not started yet.
        /// </summary>
        Unstarted,
        /// <summary>
        /// Tournament is currently being simulated.
        /// </summary>
        Running,
        /// <summary>
        /// Tournament has already finished.
        /// </summary>
        Finished,
        /// <summary>
        /// Tournament was closed because there were not enought players signed at start time.
        /// </summary>
        NotEnoughtPlayersClosed
    }
}