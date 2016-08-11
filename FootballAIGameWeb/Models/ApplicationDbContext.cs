using System.Collections;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FootballAIGameWeb.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        // db sets
        public DbSet<Player> Players { get; set; }

        public DbSet<Match> Matches { get; set; }

        public DbSet<Tournament> Tournaments { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }
    }
}