using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidMark.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class addadtranactionLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OffCode_Users_UserId",
                table: "OffCode");

            migrationBuilder.DropIndex(
                name: "IX_OffCode_UserId",
                table: "OffCode");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OffCode");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "PaymentRefId",
                table: "Orders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 10, 13, 39, 57, 538, DateTimeKind.Local).AddTicks(2831));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 10, 13, 39, 57, 538, DateTimeKind.Local).AddTicks(2844));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 10, 13, 39, 57, 538, DateTimeKind.Local).AddTicks(3251));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 10, 13, 39, 57, 538, DateTimeKind.Local).AddTicks(3253));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "EmailActiveCode" },
                values: new object[] { new DateTime(2025, 9, 10, 13, 39, 57, 538, DateTimeKind.Local).AddTicks(3215), "ed9a717b-0565-4672-b8e4-f3427a737a75" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentRefId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Orders");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "OffCode",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 9, 7, 6, 26, 883, DateTimeKind.Local).AddTicks(8817));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 9, 7, 6, 26, 883, DateTimeKind.Local).AddTicks(8836));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 9, 7, 6, 26, 883, DateTimeKind.Local).AddTicks(9193));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 9, 7, 6, 26, 883, DateTimeKind.Local).AddTicks(9195));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "EmailActiveCode" },
                values: new object[] { new DateTime(2025, 9, 9, 7, 6, 26, 883, DateTimeKind.Local).AddTicks(9167), "55282cc8-565a-46a7-b931-20177e170491" });

            migrationBuilder.CreateIndex(
                name: "IX_OffCode_UserId",
                table: "OffCode",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OffCode_Users_UserId",
                table: "OffCode",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
