using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Space.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RecClasses_Name",
                table: "RecClasses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NameTypes_Name",
                table: "NameTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meteorites_Year",
                table: "Meteorites",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_Meteorites_Year_RecClassId",
                table: "Meteorites",
                columns: new[] { "Year", "RecClassId" });

            migrationBuilder.CreateIndex(
                name: "IX_GeolocationTypes_Name",
                table: "GeolocationTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecClasses_Name",
                table: "RecClasses");

            migrationBuilder.DropIndex(
                name: "IX_NameTypes_Name",
                table: "NameTypes");

            migrationBuilder.DropIndex(
                name: "IX_Meteorites_Year",
                table: "Meteorites");

            migrationBuilder.DropIndex(
                name: "IX_Meteorites_Year_RecClassId",
                table: "Meteorites");

            migrationBuilder.DropIndex(
                name: "IX_GeolocationTypes_Name",
                table: "GeolocationTypes");
        }
    }
}
