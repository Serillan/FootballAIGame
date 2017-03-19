﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootballAIGameWeb.Models
{
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Tournament"/> class.
        /// </summary>
        public Tournament()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tournament"/> class.
        /// </summary>
        /// <param name="reccuringTournament">The reccuring tournament.</param>
        /// <param name="startTime">The start time.</param>
        public Tournament(RecurringTournament reccuringTournament, DateTime startTime)
        {
            StartTime = startTime;
            Name = reccuringTournament.Name;
            MinimumNumberOfPlayers = reccuringTournament.MinimumNumberOfPlayers;
            MaximumNumberOfPlayers = reccuringTournament.MaximumNumberOfPlayers;
            RecurringTournament = reccuringTournament;
        }
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
        NotEnoughtPlayersClosed,
        /// <summary>
        /// Tournament was closed because there there was an error during it's simulation.
        /// </summary>
        ErrorClosed
    }
}