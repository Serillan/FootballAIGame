namespace FootballAIGame.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccessKeyToPlayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "AccessKey", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Players", "AccessKey");
        }
    }
}
