namespace Vertragsmanagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Contracts", name: "contractSpeciesID", newName: "speciesID");
            RenameIndex(table: "dbo.Contracts", name: "IX_contractSpeciesID", newName: "IX_speciesID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Contracts", name: "IX_speciesID", newName: "IX_contractSpeciesID");
            RenameColumn(table: "dbo.Contracts", name: "speciesID", newName: "contractSpeciesID");
        }
    }
}
