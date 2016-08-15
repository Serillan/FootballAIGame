namespace FootballAIGameServer
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<Challenge> Challenges { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Tournament> Tournaments { get; set; }
        /*
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base();
            modelBuilder.Entity<Player>()
                .HasMany(e => e.Challenges)
                .WithOptional(e => e.Player)
                .HasForeignKey(e => e.ChallengedPlayer_UserId);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.Challenges1)
                .WithOptional(e => e.Player1)
                .HasForeignKey(e => e.ChallengingPlayer_UserId);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.Matches)
                .WithOptional(e => e.Player)
                .HasForeignKey(e => e.Player1_UserId);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.Matches1)
                .WithOptional(e => e.Player1)
                .HasForeignKey(e => e.Player2_UserId);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.Tournaments)
                .WithMany(e => e.Players)
                .Map(m => m.ToTable("TournamentPlayers"));
        }
        */
    }
}
