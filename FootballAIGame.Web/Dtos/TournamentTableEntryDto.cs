using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGame.Web.Controllers.Api;
using FootballAIGame.DbModel.Models;

namespace FootballAIGame.Web.Dtos
{
    /// <summary>
    /// <see cref="Tournament"/> data access object that is returned from the <see cref="TournamentsController.GetJoinedTournaments"/> service.
    /// </summary>
    public class TournamentTableEntryDto
    {
        /// <summary>
        /// Gets or sets the tournament identifier.
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
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the start time of the tournament.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the state of the tournament.
        /// </summary>
        /// <value>
        /// The state of the tournament.
        /// </value>
        public TournamentState TournamentState { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of players that can participate in the tournament.
        /// </summary>
        /// <value>
        /// The maximum number of players.
        /// </value>
        public int MaximumNumberOfPlayers { get; set; }

        /// <summary>
        /// Gets or sets the current number of players that are participating in the tournament.
        /// </summary>
        /// <value>
        /// The current number of players.
        /// </value>
        public int CurrentNumberOfPlayers { get; set; }

        /// <summary>
        /// Gets or sets the current player joined AI name.
        /// </summary>
        /// <value>
        /// The current player joined AI name.
        /// </value>
        public string CurrentPlayerJoinedAi { get; set; }
    }
}