using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class AddPlayerState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "PlayerState", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Players", "PlayerState");
        }
    }
}
