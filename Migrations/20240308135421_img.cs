using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bitirme.Migrations
{
    /// <inheritdoc />
    public partial class img : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductImage",
                table: "tbl_products");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "tbl_products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "tbl_products");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProductImage",
                table: "tbl_products",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
