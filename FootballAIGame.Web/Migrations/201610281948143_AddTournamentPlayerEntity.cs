using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class AddTournamentPlayerEntity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TournamentPlayers", "Tournament_Id", "dbo.Tournaments");
            DropForeignKey("dbo.TournamentPlayers", "Player_UserId", "dbo.Players");
            DropIndex("dbo.TournamentPlayers", new[] { "Tournament_Id" });
            DropIndex("dbo.TournamentPlayers", new[] { "Player_UserId" });
            DropTable("dbo.TournamentPlayers");

            CreateTable(
                "dbo.TournamentPlayers",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        TournamentId = c.Int(nullable: false),
                        PlayerPosition = c.Int(),
                    })
                .PrimaryKey(t => new { t.UserId, t.TournamentId })
                .ForeignKey("dbo.Players", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.TournamentId);
            
        }

        public override void Down()
        {
            DropForeignKey("dbo.TournamentPlayers", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.TournamentPlayers", "UserId", "dbo.Players");
            DropIndex("dbo.TournamentPlayers", new[] { "TournamentId" });
            DropIndex("dbo.TournamentPlayers", new[] { "UserId" });
            DropTable("dbo.TournamentPlayers");

            CreateTable(
                "dbo.TournamentPlayers",
                c => new
                    {
                        Tournament_Id = c.Int(nullable: false),
                        Player_UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Tournament_Id, t.Player_UserId });
            
            CreateIndex("dbo.TournamentPlayers", "Player_UserId");
            CreateIndex("dbo.TournamentPlayers", "Tournament_Id");
            AddForeignKey("dbo.TournamentPlayers", "Player_UserId", "dbo.Players", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.TournamentPlayers", "Tournament_Id", "dbo.Tournaments", "Id", cascadeDelete: true);
        }
    }
}
