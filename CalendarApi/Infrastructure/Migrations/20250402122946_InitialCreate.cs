using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Begin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Begin", "Description", "End", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 4, 2, 13, 29, 45, 990, DateTimeKind.Local).AddTicks(666), "Evt A description", new DateTime(2025, 4, 2, 14, 29, 45, 990, DateTimeKind.Local).AddTicks(793), "Event A" },
                    { 2, new DateTime(2025, 4, 2, 14, 29, 45, 990, DateTimeKind.Local).AddTicks(798), "Evt B description", new DateTime(2025, 4, 2, 15, 29, 45, 990, DateTimeKind.Local).AddTicks(799), "Event B" },
                    { 3, new DateTime(2025, 4, 2, 15, 29, 45, 990, DateTimeKind.Local).AddTicks(801), "Evt B description", new DateTime(2025, 4, 2, 16, 29, 45, 990, DateTimeKind.Local).AddTicks(803), "Event C" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
