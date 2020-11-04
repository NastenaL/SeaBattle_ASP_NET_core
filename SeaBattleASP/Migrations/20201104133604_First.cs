using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SeaBattleASP.Migrations
{
    public partial class First : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cells",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    X = table.Column<int>(nullable: false),
                    Y = table.Column<int>(nullable: false),
                    Color = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Decks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    State = table.Column<int>(nullable: false),
                    IsHead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayingField",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Width = table.Column<int>(nullable: false),
                    Heigth = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayingField", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuxiliaryShips",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Range = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: true),
                    IsSelectedShip = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuxiliaryShips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuxiliaryShips_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MilitaryShips",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Range = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: true),
                    IsSelectedShip = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MilitaryShips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MilitaryShips_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MixShips",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Range = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: true),
                    IsSelectedShip = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixShips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MixShips_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PlayingFieldId = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    Player1Id = table.Column<int>(nullable: true),
                    Player2Id = table.Column<int>(nullable: true),
                    IsPl1Turn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Players_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Players_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_PlayingField_PlayingFieldId",
                        column: x => x.PlayingFieldId,
                        principalTable: "PlayingField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeckCells",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeckId = table.Column<int>(nullable: true),
                    CellId = table.Column<int>(nullable: true),
                    AuxiliaryShipId = table.Column<int>(nullable: true),
                    MilitaryShipId = table.Column<int>(nullable: true),
                    MixShipId = table.Column<int>(nullable: true),
                    PlayingFieldId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckCells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeckCells_AuxiliaryShips_AuxiliaryShipId",
                        column: x => x.AuxiliaryShipId,
                        principalTable: "AuxiliaryShips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeckCells_Cells_CellId",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeckCells_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeckCells_MilitaryShips_MilitaryShipId",
                        column: x => x.MilitaryShipId,
                        principalTable: "MilitaryShips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeckCells_MixShips_MixShipId",
                        column: x => x.MixShipId,
                        principalTable: "MixShips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeckCells_PlayingField_PlayingFieldId",
                        column: x => x.PlayingFieldId,
                        principalTable: "PlayingField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuxiliaryShips_PlayerId",
                table: "AuxiliaryShips",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCells_AuxiliaryShipId",
                table: "DeckCells",
                column: "AuxiliaryShipId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCells_CellId",
                table: "DeckCells",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCells_DeckId",
                table: "DeckCells",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCells_MilitaryShipId",
                table: "DeckCells",
                column: "MilitaryShipId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCells_MixShipId",
                table: "DeckCells",
                column: "MixShipId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckCells_PlayingFieldId",
                table: "DeckCells",
                column: "PlayingFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player1Id",
                table: "Games",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player2Id",
                table: "Games",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_PlayingFieldId",
                table: "Games",
                column: "PlayingFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_MilitaryShips_PlayerId",
                table: "MilitaryShips",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_MixShips_PlayerId",
                table: "MixShips",
                column: "PlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeckCells");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "AuxiliaryShips");

            migrationBuilder.DropTable(
                name: "Cells");

            migrationBuilder.DropTable(
                name: "Decks");

            migrationBuilder.DropTable(
                name: "MilitaryShips");

            migrationBuilder.DropTable(
                name: "MixShips");

            migrationBuilder.DropTable(
                name: "PlayingField");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
