using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentDJ.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanceRounds",
                columns: table => new
                {
                    DanceRoundId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    MinDifficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxDifficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    MinCharacteristics = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanceRounds", x => x.DanceRoundId);
                });

            migrationBuilder.CreateTable(
                name: "Dances",
                columns: table => new
                {
                    DanceTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    MinBPM = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxBPM = table.Column<int>(type: "INTEGER", nullable: false),
                    DanceIdentifiers = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dances", x => x.DanceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "TrackLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderElementsDance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderRank = table.Column<int>(type: "INTEGER", nullable: false),
                    ObjectToOrderDanceTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    DanceRoundId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderElementsDance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderElementsDance_DanceRounds_DanceRoundId",
                        column: x => x.DanceRoundId,
                        principalTable: "DanceRounds",
                        principalColumn: "DanceRoundId");
                    table.ForeignKey(
                        name: "FK_OrderElementsDance_Dances_ObjectToOrderDanceTypeId",
                        column: x => x.ObjectToOrderDanceTypeId,
                        principalTable: "Dances",
                        principalColumn: "DanceTypeId");
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DanceTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    LastPlayedTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FlaggedAsFavourite = table.Column<bool>(type: "INTEGER", nullable: false),
                    FlaggedForReview = table.Column<bool>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    Characteristic = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    Genre = table.Column<string>(type: "TEXT", nullable: true),
                    ISRC = table.Column<string>(type: "TEXT", nullable: true),
                    Album = table.Column<string>(type: "TEXT", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    BeatsPerMinute = table.Column<uint>(type: "INTEGER", nullable: true),
                    Uris = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tracks_Dances_DanceTypeId",
                        column: x => x.DanceTypeId,
                        principalTable: "Dances",
                        principalColumn: "DanceTypeId");
                });

            migrationBuilder.CreateTable(
                name: "TrackTrackList",
                columns: table => new
                {
                    TrackListsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TracksId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackTrackList", x => new { x.TrackListsId, x.TracksId });
                    table.ForeignKey(
                        name: "FK_TrackTrackList_TrackLists_TrackListsId",
                        column: x => x.TrackListsId,
                        principalTable: "TrackLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackTrackList_Tracks_TracksId",
                        column: x => x.TracksId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderElementsDance_DanceRoundId",
                table: "OrderElementsDance",
                column: "DanceRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderElementsDance_ObjectToOrderDanceTypeId",
                table: "OrderElementsDance",
                column: "ObjectToOrderDanceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_DanceTypeId",
                table: "Tracks",
                column: "DanceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackTrackList_TracksId",
                table: "TrackTrackList",
                column: "TracksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderElementsDance");

            migrationBuilder.DropTable(
                name: "TrackTrackList");

            migrationBuilder.DropTable(
                name: "DanceRounds");

            migrationBuilder.DropTable(
                name: "TrackLists");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "Dances");
        }
    }
}
