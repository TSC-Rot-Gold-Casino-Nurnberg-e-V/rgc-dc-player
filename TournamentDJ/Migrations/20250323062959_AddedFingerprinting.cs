using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentDJ.Migrations
{
    /// <inheritdoc />
    public partial class AddedFingerprinting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Artist",
                table: "Tracks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Artist",
                table: "Tracks");
        }
    }
}
