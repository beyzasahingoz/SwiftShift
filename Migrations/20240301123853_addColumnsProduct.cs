using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bitirme.Migrations
{
    /// <inheritdoc />
    public partial class addColumnsProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "tbl_products",
                newName: "ToDistrictId");

            migrationBuilder.RenameColumn(
                name: "CityId",
                table: "tbl_products",
                newName: "ToCountryId");

            migrationBuilder.AlterColumn<string>(
                name: "ProductNote",
                table: "tbl_products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "tbl_products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductKg",
                table: "tbl_products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "tbl_products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "FromCityId",
                table: "tbl_products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FromCountryId",
                table: "tbl_products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FromDistrictId",
                table: "tbl_products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ToCityId",
                table: "tbl_products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "WhenDate",
                table: "tbl_products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_tbl_products_FromCityId",
                table: "tbl_products",
                column: "FromCityId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_products_FromCountryId",
                table: "tbl_products",
                column: "FromCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_products_FromDistrictId",
                table: "tbl_products",
                column: "FromDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_products_ToCityId",
                table: "tbl_products",
                column: "ToCityId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_products_ToCountryId",
                table: "tbl_products",
                column: "ToCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_products_ToDistrictId",
                table: "tbl_products",
                column: "ToDistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_products_Cities_FromCityId",
                table: "tbl_products",
                column: "FromCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_products_Cities_ToCityId",
                table: "tbl_products",
                column: "ToCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_products_Countries_FromCountryId",
                table: "tbl_products",
                column: "FromCountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_products_Countries_ToCountryId",
                table: "tbl_products",
                column: "ToCountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_products_District_FromDistrictId",
                table: "tbl_products",
                column: "FromDistrictId",
                principalTable: "District",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_products_District_ToDistrictId",
                table: "tbl_products",
                column: "ToDistrictId",
                principalTable: "District",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_products_Cities_FromCityId",
                table: "tbl_products");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_products_Cities_ToCityId",
                table: "tbl_products");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_products_Countries_FromCountryId",
                table: "tbl_products");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_products_Countries_ToCountryId",
                table: "tbl_products");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_products_District_FromDistrictId",
                table: "tbl_products");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_products_District_ToDistrictId",
                table: "tbl_products");

            migrationBuilder.DropIndex(
                name: "IX_tbl_products_FromCityId",
                table: "tbl_products");

            migrationBuilder.DropIndex(
                name: "IX_tbl_products_FromCountryId",
                table: "tbl_products");

            migrationBuilder.DropIndex(
                name: "IX_tbl_products_FromDistrictId",
                table: "tbl_products");

            migrationBuilder.DropIndex(
                name: "IX_tbl_products_ToCityId",
                table: "tbl_products");

            migrationBuilder.DropIndex(
                name: "IX_tbl_products_ToCountryId",
                table: "tbl_products");

            migrationBuilder.DropIndex(
                name: "IX_tbl_products_ToDistrictId",
                table: "tbl_products");

            migrationBuilder.DropColumn(
                name: "FromCityId",
                table: "tbl_products");

            migrationBuilder.DropColumn(
                name: "FromCountryId",
                table: "tbl_products");

            migrationBuilder.DropColumn(
                name: "FromDistrictId",
                table: "tbl_products");

            migrationBuilder.DropColumn(
                name: "ToCityId",
                table: "tbl_products");

            migrationBuilder.DropColumn(
                name: "WhenDate",
                table: "tbl_products");

            migrationBuilder.RenameColumn(
                name: "ToDistrictId",
                table: "tbl_products",
                newName: "CountryId");

            migrationBuilder.RenameColumn(
                name: "ToCountryId",
                table: "tbl_products",
                newName: "CityId");

            migrationBuilder.AlterColumn<string>(
                name: "ProductNote",
                table: "tbl_products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "tbl_products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductKg",
                table: "tbl_products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "tbl_products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
