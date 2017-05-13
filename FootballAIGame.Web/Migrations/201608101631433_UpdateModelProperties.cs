using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class UpdateModelProperties : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Matches", "Player1Id");
            DropColumn("dbo.Matches", "Player2Id");
        }

        public override void Down()
        {
            AddColumn("dbo.Matches", "Player2Id", c => c.String());
            AddColumn("dbo.Matches", "Player1Id", c => c.String());
        }
    }
}
