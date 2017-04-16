using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAIGame.DbModel.Models
{
    /// <summary>
    /// Represents the relation entity between <see cref="Player"/>
    /// and <see cref="Tournament"/>. It also holds it's own properties
    /// corresponding to this relation.
    /// </summary>
    public class TournamentPlayer
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        [Key, ForeignKey("Player"), Column(Order = 0)]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the tournament identifier.
        /// </summary>
        [Key, ForeignKey("Tournament"), Column(Order = 1)]
        public int TournamentId { get; set; }

        /// <summary>
        /// Gets or sets the player.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Gets or sets the player AI with which the player is participating in the tournament.
        /// </summary>
        public string PlayerAi { get; set; }

        /// <summary>
        /// Gets or sets the tournament.
        /// </summary>
        /// <value>
        /// The tournament.
        /// </value>
        public Tournament Tournament { get; set; }

        /// <summary>
        /// Gets or sets the player position in this tournament.
        /// It is null until the tournament has been finished.
        /// If the player has left the tournament during simulation then it
        /// will still be null.
        /// </summary>
        public int? PlayerPosition { get; set; }
    }
}