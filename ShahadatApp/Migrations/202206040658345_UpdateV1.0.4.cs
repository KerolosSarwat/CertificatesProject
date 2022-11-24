namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV104 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Talab", "ServiceType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Talab", "ServiceType");
        }
    }
}
