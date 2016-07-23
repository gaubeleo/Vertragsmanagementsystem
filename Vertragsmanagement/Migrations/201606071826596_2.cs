namespace Vertragsmanagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "category", c => c.String());
            AddColumn("dbo.Documents", "contractID", c => c.Int(nullable: false));
            DropColumn("dbo.Documents", "remark");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "remark", c => c.String());
            DropColumn("dbo.Documents", "contractID");
            DropColumn("dbo.Documents", "category");
        }
    }
}
