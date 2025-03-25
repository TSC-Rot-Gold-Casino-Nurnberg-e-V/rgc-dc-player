using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentDJ.Migrations
{
    /// <inheritdoc />
    public partial class addedFingerprintHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "_avHashes",
                table: "Tracks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_avHashes",
                table: "Tracks");
        }
    }
}
