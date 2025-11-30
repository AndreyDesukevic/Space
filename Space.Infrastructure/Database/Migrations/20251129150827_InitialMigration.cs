using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Space.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeolocationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeolocationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NameTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meteorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameTypeId = table.Column<int>(type: "integer", nullable: true),
                    RecClassId = table.Column<int>(type: "integer", nullable: true),
                    Mass = table.Column<double>(type: "double precision", nullable: true),
                    Fall = table.Column<bool>(type: "boolean", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    RegionDistrictRaw = table.Column<string>(type: "text", nullable: true),
                    RegionGeoZoneRaw = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meteorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meteorites_NameTypes_NameTypeId",
                        column: x => x.NameTypeId,
                        principalTable: "NameTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Meteorites_RecClasses_RecClassId",
                        column: x => x.RecClassId,
                        principalTable: "RecClasses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Geolocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GeolocationTypeId = table.Column<int>(type: "integer", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", precision: 18, scale: 6, nullable: true),
                    Longitude = table.Column<double>(type: "double precision", precision: 18, scale: 6, nullable: true),
                    MeteoriteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geolocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Geolocations_GeolocationTypes_GeolocationTypeId",
                        column: x => x.GeolocationTypeId,
                        principalTable: "GeolocationTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Geolocations_Meteorites_MeteoriteId",
                        column: x => x.MeteoriteId,
                        principalTable: "Meteorites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Geolocations_GeolocationTypeId",
                table: "Geolocations",
                column: "GeolocationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Geolocations_MeteoriteId",
                table: "Geolocations",
                column: "MeteoriteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meteorites_NameTypeId",
                table: "Meteorites",
                column: "NameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Meteorites_RecClassId",
                table: "Meteorites",
                column: "RecClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Geolocations");

            migrationBuilder.DropTable(
                name: "GeolocationTypes");

            migrationBuilder.DropTable(
                name: "Meteorites");

            migrationBuilder.DropTable(
                name: "NameTypes");

            migrationBuilder.DropTable(
                name: "RecClasses");
        }
    }
}
