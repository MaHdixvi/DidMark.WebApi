using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidMark.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class FaqTerm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAcceptedTerms",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TermsAcceptedDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 25, 11, 7, 11, 515, DateTimeKind.Local).AddTicks(2024));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 25, 11, 7, 11, 515, DateTimeKind.Local).AddTicks(2041));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 25, 11, 7, 11, 515, DateTimeKind.Local).AddTicks(2504));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 25, 11, 7, 11, 515, DateTimeKind.Local).AddTicks(2506));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "EmailActiveCode", "HasAcceptedTerms", "TermsAcceptedDate" },
                values: new object[] { new DateTime(2025, 9, 25, 11, 7, 11, 515, DateTimeKind.Local).AddTicks(2475), "adde2bec-f3b7-4424-8224-ed25b8ea82b4", false, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAcceptedTerms",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TermsAcceptedDate",
                table: "Users");

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
