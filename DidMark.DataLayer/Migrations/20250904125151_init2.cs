using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidMark.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OffCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 4, 16, 21, 50, 737, DateTimeKind.Local).AddTicks(730));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 4, 16, 21, 50, 737, DateTimeKind.Local).AddTicks(753));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 4, 16, 21, 50, 737, DateTimeKind.Local).AddTicks(1454));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 4, 16, 21, 50, 737, DateTimeKind.Local).AddTicks(1458));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "EmailActiveCode" },
                values: new object[] { new DateTime(2025, 9, 4, 16, 21, 50, 737, DateTimeKind.Local).AddTicks(1391), "02dc076b-463a-44e3-9d43-611d10f3042d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OffCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 4, 15, 33, 50, 902, DateTimeKind.Local).AddTicks(5592));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 4, 15, 33, 50, 902, DateTimeKind.Local).AddTicks(5607));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 4, 15, 33, 50, 902, DateTimeKind.Local).AddTicks(6021));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 4, 15, 33, 50, 902, DateTimeKind.Local).AddTicks(6076));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "EmailActiveCode" },
                values: new object[] { new DateTime(2025, 9, 4, 15, 33, 50, 902, DateTimeKind.Local).AddTicks(5996), "237b000f-6f99-4d9b-bbad-13f5281f4034" });
        }
    }
}
