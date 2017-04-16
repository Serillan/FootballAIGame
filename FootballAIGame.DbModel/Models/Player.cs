using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FootballAIGame.DbModel.Utilities;

namespace FootballAIGame.DbModel.Models
{
    public class Player
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [Key, ForeignKey("User")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the identity user corresponding to this player.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the access key.
        /// </summary>
        /// <value>
        /// The access key.
        /// </value>
        public string AccessKey { get; set; }

        /// <summary>
        /// Gets or sets the tournaments in which the player has participated.
        /// </summary>
        /// <value>
        /// The tournaments in which the player has participated.
        /// </value>
        public ICollection<TournamentPlayer> Tournaments { get; set; }

        /// <summary>
        /// Gets or sets the AIs applications connected to the game server associated with the player. <para />
        /// Format "ai1Name;ai2Name;...", where ';' is separator.
        /// </summary>
        /// <value>
        /// The active AIs.
        /// </value>
        public string ActiveAis { get; set; }

        /// <summary>
        /// Gets or sets the selected AI. This AI is selected from active AIs and used for
        /// matches.
        /// </summary>
        /// <value>
        /// The selected AI.
        /// </value>
        public string SelectedAi { get; set; }

        /// <summary>
        /// Gets or sets the state of the player.
        /// </summary>
        /// <value>
        /// The state of the player.
        /// </value>
        /// <seealso cref="PlayerState"/>
        [Required]
        public PlayerState PlayerState { get; set; }

        /// <summary>
        /// Gets or sets player score. Player's score is increased by winning tournaments.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the number of won games.
        /// </summary>
        /// <value>
        /// The number of won games.
        /// </value>
        public int WonGames { get; set; }

        /// <summary>
        /// Gets or sets the number of won tournaments.
        /// </summary>
        /// <value>
        /// The number of won tournaments.
        /// </value>
        public int WonTournaments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        public Player(User user)
        {
            UserId = user.Id;
            Name = user.UserName;
            PlayerState = PlayerState.Idle;
            Score = 0;
            WonGames = 0;
            WonTournaments = 0;
            AccessKey = AccessKeyGenerator.Generate();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// Parameterless constructor must be present for entity framework to work correctly!
        /// </summary>
        public Player()
        {
        }
    }
}