using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMusicVectorNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE [AspNetUsers] ALTER COLUMN [MusicVector] nvarchar(max) NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [AspNetUsers] SET [MusicVector] = '' WHERE [MusicVector] IS NULL;");
            migrationBuilder.Sql("ALTER TABLE [AspNetUsers] ALTER COLUMN [MusicVector] nvarchar(max) NOT NULL;");
        }
    }
}