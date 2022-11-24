namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV108 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Area", "MorfkatNum", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Area", "MorfkatNum");
        }
    }
}
