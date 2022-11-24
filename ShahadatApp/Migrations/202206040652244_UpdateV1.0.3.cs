namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV103 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Talab", "ServiceType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Talab", "ServiceType", c => c.String());
        }
    }
}
