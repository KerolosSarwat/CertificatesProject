namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV114 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Citizen", "MorfkatSent", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Citizen", "MorfkatSent");
        }
    }
}
