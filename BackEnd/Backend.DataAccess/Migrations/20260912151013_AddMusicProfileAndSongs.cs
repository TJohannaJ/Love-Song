using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMusicProfileAndSongs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MusicVector",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    SpotifyTrackId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Artist = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviewUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valence = table.Column<float>(type: "real", nullable: false),
                    Energy = table.Column<float>(type: "real", nullable: false),
                    Danceability = table.Column<float>(type: "real", nullable: false),
                    Acousticness = table.Column<float>(type: "real", nullable: false),
                    NormalizedBpm = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.SpotifyTrackId);
                });

            migrationBuilder.CreateTable(
                name: "UserTopSongs",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SpotifyTrackId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTopSongs", x => new { x.UserId, x.SpotifyTrackId });
                    table.ForeignKey(
                        name: "FK_UserTopSongs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTopSongs_Songs_SpotifyTrackId",
                        column: x => x.SpotifyTrackId,
                        principalTable: "Songs",
                        principalColumn: "SpotifyTrackId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTopSongs_SpotifyTrackId",
                table: "UserTopSongs",
                column: "SpotifyTrackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTopSongs");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropColumn(
                name: "MusicVector",
                table: "AspNetUsers");
        }
    }
}
