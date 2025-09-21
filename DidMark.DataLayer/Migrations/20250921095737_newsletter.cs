using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidMark.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class newsletter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "newsletterSubscribers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_newsletterSubscribers", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 13, 27, 36, 524, DateTimeKind.Local).AddTicks(5215));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 13, 27, 36, 524, DateTimeKind.Local).AddTicks(5230));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 13, 27, 36, 524, DateTimeKind.Local).AddTicks(5856));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 13, 27, 36, 524, DateTimeKind.Local).AddTicks(5857));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "EmailActiveCode" },
                values: new object[] { new DateTime(2025, 9, 21, 13, 27, 36, 524, DateTimeKind.Local).AddTicks(5829), "1633a8d3-ea87-4db1-86e4-edad6ceda3d4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "newsletterSubscribers");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 13, 2, 7, 87, DateTimeKind.Local).AddTicks(7766));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 13, 2, 7, 87, DateTimeKind.Local).AddTicks(7779));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 13, 2, 7, 87, DateTimeKind.Local).AddTicks(8187));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 13, 2, 7, 87, DateTimeKind.Local).AddTicks(8188));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "EmailActiveCode" },
                values: new object[] { new DateTime(2025, 9, 21, 13, 2, 7, 87, DateTimeKind.Local).AddTicks(8157), "d46a2128-97eb-46d8-be12-9646e4ef3c17" });
        }
    }
}
