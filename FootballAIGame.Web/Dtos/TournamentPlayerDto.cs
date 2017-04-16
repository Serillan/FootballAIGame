using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGame.Web.Controllers.Api;
using FootballAIGame.DbModel.Models;

namespace FootballAIGame.Web.Dtos
{
    /// <summary>
    /// <see cref="TournamentPlayer"/> data access object that is returned from the <see cref="GameController.GetTournamentInfo"/> service as part of the
    /// <see cref="TournamentInfoDto"/>.
    /// </summary>
    public class TournamentPlayerDto
    {
        /// <summary>
        /// Gets or sets the player position in this tournament.
        /// It is null until the tournament has been finished.
        /// If the player has left the tournament during simulation then it
        /// will still be null.
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// Gets or sets the player name.
        /// </summary>
        /// <value>
        /// The player name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets player score. Player's score is increased by winning tournaments.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score { get; set; }
    }
}