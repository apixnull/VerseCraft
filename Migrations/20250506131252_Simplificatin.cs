using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VerseCraft.Migrations
{
    /// <inheritdoc />
    public partial class Simplificatin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSavedAnthologies",
                columns: table => new
                {
                    SavedAnthologiesId = table.Column<int>(type: "INTEGER", nullable: false),
                    User1Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSavedAnthologies", x => new { x.SavedAnthologiesId, x.User1Id });
                    table.ForeignKey(
                        name: "FK_UserSavedAnthologies_Anthologies_SavedAnthologiesId",
                        column: x => x.SavedAnthologiesId,
                        principalTable: "Anthologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSavedAnthologies_Users_User1Id",
                        column: x => x.User1Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSavedPoems",
                columns: table => new
                {
                    SavedPoemsId = table.Column<int>(type: "INTEGER", nullable: false),
                    User1Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSavedPoems", x => new { x.SavedPoemsId, x.User1Id });
                    table.ForeignKey(
                        name: "FK_UserSavedPoems_Poems_SavedPoemsId",
                        column: x => x.SavedPoemsId,
                        principalTable: "Poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSavedPoems_Users_User1Id",
                        column: x => x.User1Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedAnthologies_User1Id",
                table: "UserSavedAnthologies",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedPoems_User1Id",
                table: "UserSavedPoems",
                column: "User1Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSavedAnthologies");

            migrationBuilder.DropTable(
                name: "UserSavedPoems");
        }
    }
}
