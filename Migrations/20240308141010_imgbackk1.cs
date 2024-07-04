using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bitirme.Migrations
{
    /// <inheritdoc />
    public partial class imgbackk1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "path",
                table: "tbl_products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "path",
                table: "tbl_products");
        }
    }
}
