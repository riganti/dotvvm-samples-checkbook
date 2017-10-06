namespace CheckBook.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentSoftDelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "IsDeleted");
        }
    }
}
