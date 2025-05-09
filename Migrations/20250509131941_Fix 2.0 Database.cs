using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VerseCraft.Migrations
{
    /// <inheritdoc />
    public partial class Fix20Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Poems_Anthologies_AnthologyId",
                table: "Poems");

            migrationBuilder.DropIndex(
                name: "IX_Poems_AnthologyId",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "AnthologyId",
                table: "Poems");

            migrationBuilder.CreateTable(
                name: "AnthologyPoems",
                columns: table => new
                {
                    AnthologyId = table.Column<int>(type: "INTEGER", nullable: false),
                    PoemId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnthologyPoems", x => new { x.AnthologyId, x.PoemId });
                    table.ForeignKey(
                        name: "FK_AnthologyPoems_Anthologies_AnthologyId",
                        column: x => x.AnthologyId,
                        principalTable: "Anthologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnthologyPoems_Poems_PoemId",
                        column: x => x.PoemId,
                        principalTable: "Poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnthologyPoems_PoemId",
                table: "AnthologyPoems",
                column: "PoemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnthologyPoems");

            migrationBuilder.AddColumn<int>(
                name: "AnthologyId",
                table: "Poems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Poems_AnthologyId",
                table: "Poems",
                column: "AnthologyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Poems_Anthologies_AnthologyId",
                table: "Poems",
                column: "AnthologyId",
                principalTable: "Anthologies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
