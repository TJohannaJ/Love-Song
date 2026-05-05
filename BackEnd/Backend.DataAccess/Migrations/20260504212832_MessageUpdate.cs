using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MessageUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Likes_LikeId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "LikeId",
                table: "Messages",
                newName: "LikeId1");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_LikeId",
                table: "Messages",
                newName: "IX_Messages_LikeId1");

            migrationBuilder.AddColumn<string>(
                name: "Like2Id",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LikeId2",
                table: "Messages",
                type: "nvarchar(max)",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Likes_LikeId1",
                table: "Messages",
                column: "LikeId1",
                principalTable: "Likes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Likes_Like2Id",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Likes_LikeId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_Like2Id",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Like2Id",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "LikeId2",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "LikeId1",
                table: "Messages",
                newName: "LikeId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_LikeId1",
                table: "Messages",
                newName: "IX_Messages_LikeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Likes_LikeId",
                table: "Messages",
                column: "LikeId",
                principalTable: "Likes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
