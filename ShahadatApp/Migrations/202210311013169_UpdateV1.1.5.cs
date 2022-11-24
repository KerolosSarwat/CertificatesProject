namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV115 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Talab", "CertificateType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Talab", "CertificateType");
        }
    }
}
