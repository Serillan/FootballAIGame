using System.Collections;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FootballAIGameServer.Models
{
    public class ApplicationDbContext : DbContext
    {
        // db sets
        public DbSet<Player> Players { get; set; }

        public DbSet<Match> Matches { get; set; }

        public DbSet<Tournament> Tournaments { get; set; }

        public DbSet<Challenge> Challenges { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

    }
}