using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class AddChallenges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Challenges",
                c => new
                    {
                        ChallengingPlayerId = c.String(nullable: false, maxLength: 128),
                        ChallengedPlayer_UserId = c.String(maxLength: 128),
                        ChallengingPlayer_UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ChallengingPlayerId)
                .ForeignKey("dbo.Players", t => t.ChallengedPlayer_UserId)
                .ForeignKey("dbo.Players", t => t.ChallengingPlayer_UserId)
                .Index(t => t.ChallengedPlayer_UserId)
                .Index(t => t.ChallengingPlayer_UserId);
            
        }

        public override void Down()
        {
            DropForeignKey("dbo.Challenges", "ChallengingPlayer_UserId", "dbo.Players");
            DropForeignKey("dbo.Challenges", "ChallengedPlayer_UserId", "dbo.Players");
            DropIndex("dbo.Challenges", new[] { "ChallengingPlayer_UserId" });
            DropIndex("dbo.Challenges", new[] { "ChallengedPlayer_UserId" });
            DropTable("dbo.Challenges");
        }
    }
}
