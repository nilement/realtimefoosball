using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ToughBattle.Migrations
{
    public partial class Identity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupPlacePair",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Group = table.Column<int>(nullable: false),
                    Place = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPlacePair", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    AvatarUrl = table.Column<string>(nullable: true),
                    Wins = table.Column<int>(nullable: false),
                    Losses = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    GroupCount = table.Column<int>(nullable: false),
                    HasEnded = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    PlayoffMatchupsCount = table.Column<int>(nullable: false),
                    StartingPhase = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    GoogleId = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TournamentPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TournamentId = table.Column<int>(nullable: true),
                    PlayerId = table.Column<int>(nullable: true),
                    GroupId = table.Column<int>(nullable: false),
                    Wins = table.Column<int>(nullable: false),
                    Losses = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TournamentPlayers_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matchups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TournamentId = table.Column<int>(nullable: true),
                    TournamentPhase = table.Column<int>(nullable: false),
                    NextMatchupId = table.Column<int>(nullable: true),
                    AdvanceToUpper = table.Column<bool>(nullable: false),
                    Finished = table.Column<bool>(nullable: false),
                    Wins1 = table.Column<int>(nullable: false),
                    Wins2 = table.Column<int>(nullable: false),
                    UpperPairId = table.Column<int>(nullable: true),
                    LowerPairId = table.Column<int>(nullable: true),
                    Merged = table.Column<bool>(nullable: false),
                    T1P1Id = table.Column<int>(nullable: true),
                    T1P2Id = table.Column<int>(nullable: true),
                    T2P1Id = table.Column<int>(nullable: true),
                    T2P2Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matchups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matchups_GroupPlacePair_LowerPairId",
                        column: x => x.LowerPairId,
                        principalTable: "GroupPlacePair",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matchups_Matchups_NextMatchupId",
                        column: x => x.NextMatchupId,
                        principalTable: "Matchups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matchups_TournamentPlayers_T1P1Id",
                        column: x => x.T1P1Id,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matchups_TournamentPlayers_T1P2Id",
                        column: x => x.T1P2Id,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matchups_TournamentPlayers_T2P1Id",
                        column: x => x.T2P1Id,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matchups_TournamentPlayers_T2P2Id",
                        column: x => x.T2P2Id,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matchups_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matchups_GroupPlacePair_UpperPairId",
                        column: x => x.UpperPairId,
                        principalTable: "GroupPlacePair",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RedTeamScore = table.Column<int>(nullable: false),
                    BlueTeamScore = table.Column<int>(nullable: false),
                    PlayerType = table.Column<int>(nullable: false),
                    GameType = table.Column<int>(nullable: false),
                    TournamentMatchupId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    RP1Id = table.Column<int>(nullable: true),
                    RP2Id = table.Column<int>(nullable: true),
                    BP1Id = table.Column<int>(nullable: true),
                    BP2Id = table.Column<int>(nullable: true),
                    TournamentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Players_BP1Id",
                        column: x => x.BP1Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Players_BP2Id",
                        column: x => x.BP2Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Players_RP1Id",
                        column: x => x.RP1Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Players_RP2Id",
                        column: x => x.RP2Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Matchups_TournamentMatchupId",
                        column: x => x.TournamentMatchupId,
                        principalTable: "Matchups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ScorerId = table.Column<int>(nullable: true),
                    ReceiverId = table.Column<int>(nullable: true),
                    GameId = table.Column<int>(nullable: true),
                    Velocity = table.Column<double>(nullable: false),
                    VideoUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Goals_Players_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Goals_Players_ScorerId",
                        column: x => x.ScorerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_BP1Id",
                table: "Games",
                column: "BP1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_BP2Id",
                table: "Games",
                column: "BP2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_RP1Id",
                table: "Games",
                column: "RP1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_RP2Id",
                table: "Games",
                column: "RP2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_TournamentId",
                table: "Games",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_TournamentMatchupId",
                table: "Games",
                column: "TournamentMatchupId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_GameId",
                table: "Goals",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_ReceiverId",
                table: "Goals",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_ScorerId",
                table: "Goals",
                column: "ScorerId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_LowerPairId",
                table: "Matchups",
                column: "LowerPairId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_NextMatchupId",
                table: "Matchups",
                column: "NextMatchupId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_T1P1Id",
                table: "Matchups",
                column: "T1P1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_T1P2Id",
                table: "Matchups",
                column: "T1P2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_T2P1Id",
                table: "Matchups",
                column: "T2P1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_T2P2Id",
                table: "Matchups",
                column: "T2P2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_TournamentId",
                table: "Matchups",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_UpperPairId",
                table: "Matchups",
                column: "UpperPairId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentPlayers_PlayerId",
                table: "TournamentPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentPlayers_TournamentId",
                table: "TournamentPlayers",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PlayerId",
                table: "Users",
                column: "PlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Matchups");

            migrationBuilder.DropTable(
                name: "GroupPlacePair");

            migrationBuilder.DropTable(
                name: "TournamentPlayers");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Tournaments");
        }
    }
}
