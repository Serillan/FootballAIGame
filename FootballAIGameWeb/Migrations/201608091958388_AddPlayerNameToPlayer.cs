namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlayerNameToPlayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "Name");
        }
    }
}
