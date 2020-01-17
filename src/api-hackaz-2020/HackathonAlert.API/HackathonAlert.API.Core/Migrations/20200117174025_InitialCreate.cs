using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HackathonAlert.API.Core.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AlarmId = table.Column<string>(nullable: true),
                    StreamId = table.Column<string>(nullable: true),
                    SourceName = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Region = table.Column<int>(nullable: false),
                    TimePosted = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Altitude = table.Column<double>(nullable: false),
                    Heading = table.Column<double>(nullable: false),
                    Velocity = table.Column<double>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ParentAlertId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Alerts_ParentAlertId",
                        column: x => x.ParentAlertId,
                        principalTable: "Alerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_TimePosted",
                table: "Alerts",
                column: "TimePosted");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_ParentAlertId",
                table: "Positions",
                column: "ParentAlertId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Alerts");
        }
    }
}
