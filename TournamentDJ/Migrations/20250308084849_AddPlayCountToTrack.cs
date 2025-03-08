using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentDJ.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayCountToTrack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayCount",
                table: "Tracks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayCount",
                table: "Tracks");
        }
    }
}
