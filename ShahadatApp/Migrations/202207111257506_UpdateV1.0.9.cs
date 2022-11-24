namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV109 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notification",
                c => new
                    {
                        NotificationID = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        Message = c.String(),
                        ExtraColumn = c.String(),
                    })
                .PrimaryKey(t => t.NotificationID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Notification");
        }
    }
}
