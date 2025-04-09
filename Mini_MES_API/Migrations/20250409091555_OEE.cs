using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mini_MES_API.Migrations
{
    /// <inheritdoc />
    public partial class OEE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefectCount",
                table: "ProductionOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "IdealCycleTimeMinutes",
                table: "ProductionOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedEndTime",
                table: "ProductionOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefectCount",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "IdealCycleTimeMinutes",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "PlannedEndTime",
                table: "ProductionOrders");
        }
    }
}
