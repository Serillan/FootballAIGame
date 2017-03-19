using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class AddActiveAisPropertyToPlayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "ActiveAis", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "ActiveAis");
        }
    }
}
