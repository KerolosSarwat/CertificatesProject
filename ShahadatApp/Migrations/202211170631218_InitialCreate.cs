namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Citizen", "MorfakPics", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Citizen", "MorfakPics");
        }
    }
}
