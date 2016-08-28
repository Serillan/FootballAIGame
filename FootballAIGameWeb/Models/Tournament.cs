using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootballAIGameWeb.Models
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
        /// Gets or sets the collection of players that are participating in the tournament.
        /// </summary>
        /// <value>
        /// The players participating.
        /// </value>
        public ICollection<Player> Players { get; set; }

    }
}