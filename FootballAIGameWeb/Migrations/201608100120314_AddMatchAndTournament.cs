namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMatchAndTournament : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Player1Id = c.Int(nullable: false),
                        Player2Id = c.Int(nullable: false),
                        Player1_UserId = c.String(maxLength: 128),
                        Player2_UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.Player1_UserId)
                .ForeignKey("dbo.Players", t => t.Player2_UserId)
                .Index(t => t.Player1_UserId)
                .Index(t => t.Player2_UserId);
            
            CreateTable(
                "dbo.Tournaments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Matches", "Player2_UserId", "dbo.Players");
            DropForeignKey("dbo.Matches", "Player1_UserId", "dbo.Players");
            DropIndex("dbo.Matches", new[] { "Player2_UserId" });
            DropIndex("dbo.Matches", new[] { "Player1_UserId" });
            DropTable("dbo.Tournaments");
            DropTable("dbo.Matches");
        }
    }
}
