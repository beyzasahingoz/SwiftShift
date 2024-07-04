using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bitirme.Migrations
{
    /// <inheritdoc />
    public partial class addtraveltbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Travel",
                columns: table => new
                {
                    TravelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArrivalCountryId = table.Column<int>(type: "int", nullable: false),
                    ArrivalCityId = table.Column<int>(type: "int", nullable: false),
                    ArrivalDistrictId = table.Column<int>(type: "int", nullable: false),
                    DepartureCountryId = table.Column<int>(type: "int", nullable: false),
                    DepartureCityId = table.Column<int>(type: "int", nullable: false),
                    DepartureDistrictId = table.Column<int>(type: "int", nullable: false),
                    ArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travel", x => x.TravelId);
                    table.ForeignKey(
                        name: "FK_Travel_Cities_ArrivalCityId",
                        column: x => x.ArrivalCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Travel_Cities_DepartureCityId",
                        column: x => x.DepartureCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Travel_Countries_ArrivalCountryId",
                        column: x => x.ArrivalCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Travel_Countries_DepartureCountryId",
                        column: x => x.DepartureCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Travel_District_ArrivalDistrictId",
                        column: x => x.ArrivalDistrictId,
                        principalTable: "District",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Travel_District_DepartureDistrictId",
                        column: x => x.DepartureDistrictId,
                        principalTable: "District",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Travel_ArrivalCityId",
                table: "Travel",
                column: "ArrivalCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Travel_ArrivalCountryId",
                table: "Travel",
                column: "ArrivalCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Travel_ArrivalDistrictId",
                table: "Travel",
                column: "ArrivalDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Travel_DepartureCityId",
                table: "Travel",
                column: "DepartureCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Travel_DepartureCountryId",
                table: "Travel",
                column: "DepartureCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Travel_DepartureDistrictId",
                table: "Travel",
                column: "DepartureDistrictId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Travel");
        }
    }
}
