namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
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
