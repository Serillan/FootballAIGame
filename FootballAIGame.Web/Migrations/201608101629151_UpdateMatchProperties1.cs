using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
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
