using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnnouncementNotifier.Migrations
{
    public partial class AddAssignmentHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignmentHistory",
                columns: table => new
                {
                    AssignmentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentHistory", x => x.AssignmentId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentHistory");
        }
    }
}
