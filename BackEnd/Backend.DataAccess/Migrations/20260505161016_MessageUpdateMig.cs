using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MessageUpdateMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Likes_Like2Id",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_Like2Id",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Like2Id",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "LikeId2",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_LikeId2",
                table: "Messages",
                column: "LikeId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Likes_LikeId2",
                table: "Messages",
                column: "LikeId2",
                principalTable: "Likes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Likes_LikeId2",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_LikeId2",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "LikeId2",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Like2Id",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Like2Id",
                table: "Messages",
                column: "Like2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Likes_Like2Id",
                table: "Messages",
                column: "Like2Id",
                principalTable: "Likes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
