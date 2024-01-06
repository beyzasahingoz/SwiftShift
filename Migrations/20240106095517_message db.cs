using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bitirme.Migrations
{
    /// <inheritdoc />
    public partial class messagedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_UserID",
                table: "Messages");

           

            

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Messages",
                newName: "SenderUserName");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Messages",
                newName: "SenderUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_UserID",
                table: "Messages",
                newName: "IX_Messages_SenderUserID");

            

            

            migrationBuilder.AddColumn<string>(
                name: "ReceiverUserID",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverUserName",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

           
            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderUserID",
                table: "Messages",
                column: "SenderUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderUserID",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "tbl_products");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReceiverUserID",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReceiverUserName",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderUserName",
                table: "Messages",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "SenderUserID",
                table: "Messages",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderUserID",
                table: "Messages",
                newName: "IX_Messages_UserID");

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "tbl_products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "tbl_products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_UserID",
                table: "Messages",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
