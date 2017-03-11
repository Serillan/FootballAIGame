using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FootballAIGame.Server.Models
{
    /// <summary>
    /// The entity framework db context for interacting with database data as objects.
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext{User}" />
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        // DbSets for accessing database tables

        /// <summary>
        /// Gets or sets the players.
        /// </summary>
        /// <value>
        /// The players.
        /// </value>
        public DbSet<Player> Players { get; set; }

        /// <summary>
        /// Gets or sets the matches.
        /// </summary>
        /// <value>
        /// The matches.
        /// </value>
        public DbSet<Match> Matches { get; set; }

        /// <summary>
        /// Gets or sets the tournaments.
        /// </summary>
        /// <value>
        /// The tournaments.
        /// </value>
        public DbSet<Tournament> Tournaments { get; set; }

        /// <summary>
        /// Gets or sets the tournament players.
        /// </summary>
        /// <value>
        /// The tournament players.
        /// </value>
        public DbSet<TournamentPlayer> TournamentPlayers { get; set; }

        /// <summary>
        /// Gets or sets the recurring tournaments.
        /// </summary>
        /// <value>
        /// The recurring tournaments.
        /// </value>
        public DbSet<RecurringTournament> RecurringTournaments { get; set; }

        /// <summary>
        /// Gets or sets the challenges.
        /// </summary>
        /// <value>
        /// The challenges.
        /// </value>
        public DbSet<Challenge> Challenges { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

    }
}