using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnnouncementNotifier.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotifyHistory",
                columns: table => new
                {
                    AnnouncementId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotifyHistory", x => x.AnnouncementId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotifyHistory");
        }
    }
}
