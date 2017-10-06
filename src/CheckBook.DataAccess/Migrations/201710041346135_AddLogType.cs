namespace CheckBook.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLogType : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PaymentEdits", newName: "PaymentLogs");
            AddColumn("dbo.PaymentLogs", "LogType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentLogs", "LogType");
            RenameTable(name: "dbo.PaymentLogs", newName: "PaymentEdits");
        }
    }
}
