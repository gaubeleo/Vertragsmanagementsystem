namespace Vertragsmanagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Documents", "contractID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "contractID", c => c.Int(nullable: false));
        }
    }
}
