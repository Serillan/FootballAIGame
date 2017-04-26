using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGame.Web.Controllers.Api;
using FootballAIGame.DbModel.Models;

namespace FootballAIGame.Web.Dtos
{
    /// <summary>
    /// <see cref="Match"/> data access object that is returned from the 
    /// <see cref="TournamentsController.GetTournamentInfo"/> service as part of the
    /// <see cref="TournamentInfoDto"/>.
    /// </summary>
    public class MatchDto
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the player1.
        /// </summary>
        /// <value> 
        /// The name of the player1.
        /// </value>
        public string Player1Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the player2.
        /// </summary>
        /// <value>
        /// The name of the player2.
        /// </value>
        public string Player2Name { get; set; }

        /// <summary>
        /// Gets or sets the final match score. <para />
        /// Format: "NumberOfUser1Goals:NumberOfUser2Goals".
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public string Score { get; set; }

        /// <summary>
        /// Returns 1 if Player1 won, 2 if Player2 won, otherwise (draw) returns 0.
        /// </summary>
        /// <value>1 if Player1 won, 2 if Player2 won, otherwise (draw) returns 0.</value>
        public int Winner { get; set; }
    }
}