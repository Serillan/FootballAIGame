namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRelationshipBetweenPlayerAndTournament : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TournamentPlayers",
                c => new
                    {
                        Tournament_Id = c.Int(nullable: false),
                        Player_UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Tournament_Id, t.Player_UserId })
                .ForeignKey("dbo.Tournaments", t => t.Tournament_Id, cascadeDelete: true)
                .ForeignKey("dbo.Players", t => t.Player_UserId, cascadeDelete: true)
                .Index(t => t.Tournament_Id)
                .Index(t => t.Player_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TournamentPlayers", "Player_UserId", "dbo.Players");
            DropForeignKey("dbo.TournamentPlayers", "Tournament_Id", "dbo.Tournaments");
            DropIndex("dbo.TournamentPlayers", new[] { "Player_UserId" });
            DropIndex("dbo.TournamentPlayers", new[] { "Tournament_Id" });
            DropTable("dbo.TournamentPlayers");
        }
    }
}
