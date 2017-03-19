namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatchProperties1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Matches", "Player1Id", c => c.String());
            AlterColumn("dbo.Matches", "Player2Id", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Matches", "Player2Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Matches", "Player1Id", c => c.Int(nullable: false));
        }
    }
}
