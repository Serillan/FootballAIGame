using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGame.Web.Controllers.Api;
using FootballAIGame.Web.Models;

namespace FootballAIGame.Web.Dtos
{
    /// <summary>
    /// <see cref="Tournament"/> data access object that is returned from the <see cref="GameController.GetTournamentInfo"/> service.
    /// </summary>
    public class TournamentInfoDto
    {
        /// <summary>
        /// Gets or sets the state of the tournament.
        /// </summary>
        /// <value>
        /// The state of the tournament.
        /// </value>
        public TournamentState TournamentState { get; set; }

        /// <summary>
        /// Gets or sets the tournament's matches.
        /// </summary>
        /// <value>
        /// The tournament's matches.
        /// </value>
        public ICollection<MatchDto> Matches { get; set; }

        /// <summary>
        /// Gets or sets the collection of players that are participating in the tournament.
        /// </summary>
        /// <value>
        /// The players participating.
        /// </value>
        public ICollection<TournamentPlayerDto> Players { get; set; }
    }
}