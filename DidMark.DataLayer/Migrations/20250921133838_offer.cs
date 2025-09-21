using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidMark.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class offer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_specialOfferProducts_Products_ProductId",
                table: "specialOfferProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_specialOfferProducts_specialOffers_SpecialOfferId",
                table: "specialOfferProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_specialOffers",
                table: "specialOffers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_specialOfferProducts",
                table: "specialOfferProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_newsletterSubscribers",
                table: "newsletterSubscribers");

            migrationBuilder.RenameTable(
                name: "specialOffers",
                newName: "SpecialOffers");

            migrationBuilder.RenameTable(
                name: "specialOfferProducts",
                newName: "SpecialOfferProducts");

            migrationBuilder.RenameTable(
                name: "newsletterSubscribers",
                newName: "NewsletterSubscribers");

            migrationBuilder.RenameIndex(
                name: "IX_specialOfferProducts_SpecialOfferId",
                table: "SpecialOfferProducts",
                newName: "IX_SpecialOfferProducts_SpecialOfferId");

            migrationBuilder.RenameIndex(
                name: "IX_specialOfferProducts_ProductId",
                table: "SpecialOfferProducts",
                newName: "IX_SpecialOfferProducts_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpecialOffers",
                table: "SpecialOffers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpecialOfferProducts",
                table: "SpecialOfferProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsletterSubscribers",
                table: "NewsletterSubscribers",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialOfferProducts_Products_ProductId",
                table: "SpecialOfferProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialOfferProducts_SpecialOffers_SpecialOfferId",
                table: "SpecialOfferProducts",
                column: "SpecialOfferId",
                principalTable: "SpecialOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecialOfferProducts_Products_ProductId",
                table: "SpecialOfferProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecialOfferProducts_SpecialOffers_SpecialOfferId",
                table: "SpecialOfferProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpecialOffers",
                table: "SpecialOffers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpecialOfferProducts",
                table: "SpecialOfferProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsletterSubscribers",
                table: "NewsletterSubscribers");

            migrationBuilder.RenameTable(
                name: "SpecialOffers",
                newName: "specialOffers");

            migrationBuilder.RenameTable(
                name: "SpecialOfferProducts",
                newName: "specialOfferProducts");

            migrationBuilder.RenameTable(
                name: "NewsletterSubscribers",
                newName: "newsletterSubscribers");

            migrationBuilder.RenameIndex(
                name: "IX_SpecialOfferProducts_SpecialOfferId",
                table: "specialOfferProducts",
                newName: "IX_specialOfferProducts_SpecialOfferId");

            migrationBuilder.RenameIndex(
                name: "IX_SpecialOfferProducts_ProductId",
                table: "specialOfferProducts",
                newName: "IX_specialOfferProducts_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_specialOffers",
                table: "specialOffers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_specialOfferProducts",
                table: "specialOfferProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_newsletterSubscribers",
                table: "newsletterSubscribers",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 16, 46, 46, 199, DateTimeKind.Local).AddTicks(9252));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 16, 46, 46, 199, DateTimeKind.Local).AddTicks(9271));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 16, 46, 46, 200, DateTimeKind.Local).AddTicks(637));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreateDate",
                value: new DateTime(2025, 9, 21, 16, 46, 46, 200, DateTimeKind.Local).AddTicks(640));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "EmailActiveCode" },
                values: new object[] { new DateTime(2025, 9, 21, 16, 46, 46, 200, DateTimeKind.Local).AddTicks(567), "53a43b6e-6fcd-4f1a-9db3-542505f93211" });

            migrationBuilder.AddForeignKey(
                name: "FK_specialOfferProducts_Products_ProductId",
                table: "specialOfferProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_specialOfferProducts_specialOffers_SpecialOfferId",
                table: "specialOfferProducts",
                column: "SpecialOfferId",
                principalTable: "specialOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
