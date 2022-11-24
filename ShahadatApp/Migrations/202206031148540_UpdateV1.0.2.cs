namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV102 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Area", "Qesm", c => c.String());
            AddColumn("dbo.Area", "Mntqa", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Area", "Mntqa");
            DropColumn("dbo.Area", "Qesm");
        }
    }
}
