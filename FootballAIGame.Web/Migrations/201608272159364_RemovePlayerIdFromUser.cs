using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class RemovePlayerIdFromUser : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "PlayerId");
        }

        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "PlayerId", c => c.Int(nullable: false));
        }
    }
}
