using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidMark.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class updatedbshahmoradi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 22, 17, 51, 9, 758, DateTimeKind.Local).AddTicks(5820));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 22, 17, 51, 9, 758, DateTimeKind.Local).AddTicks(5838));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 22, 17, 51, 9, 758, DateTimeKind.Local).AddTicks(6275));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 22, 17, 51, 9, 758, DateTimeKind.Local).AddTicks(6278));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "EmailActiveCode" },
                values: new object[] { new DateTime(2025, 9, 22, 17, 51, 9, 758, DateTimeKind.Local).AddTicks(6252), "4260266a-8315-474d-9dbb-57ded44ae3a4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 17, 8, 37, 943, DateTimeKind.Local).AddTicks(6493));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 17, 8, 37, 943, DateTimeKind.Local).AddTicks(6506));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 17, 8, 37, 943, DateTimeKind.Local).AddTicks(6832));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 17, 8, 37, 943, DateTimeKind.Local).AddTicks(6833));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "EmailActiveCode" },
                values: new object[] { new DateTime(2025, 9, 21, 17, 8, 37, 943, DateTimeKind.Local).AddTicks(6811), "813ad69c-e7c6-42f3-a206-0cee45876add" });
        }
    }
}
