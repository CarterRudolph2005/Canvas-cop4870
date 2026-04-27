using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canvas.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSemesterDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SemesterTaught_EndDate",
                table: "Courses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SemesterTaught_StartDate",
                table: "Courses",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SemesterTaught_EndDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "SemesterTaught_StartDate",
                table: "Courses");
        }
    }
}
