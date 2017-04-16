
namespace FootballAIGame.DbModel.Models
{
    public class Challenge
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the challenging player.
        /// </summary>
        /// <value>
        /// The challenging player.
        /// </value>
        public Player ChallengingPlayer { get; set; }

        /// <summary>
        /// Gets or sets the challenged player.
        /// </summary>
        /// <value>
        /// The challenged player.
        /// </value>
        public Player ChallengedPlayer { get; set; }
    }
}