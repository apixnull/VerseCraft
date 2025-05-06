using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VerseCraft.Migrations
{
    /// <inheritdoc />
    public partial class aNTH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Anthologies_Users_AuthorId",
                table: "Anthologies");

            migrationBuilder.DropIndex(
                name: "IX_Anthologies_AuthorId",
                table: "Anthologies");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Anthologies");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Anthologies");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Anthologies");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Anthologies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Anthologies_AuthorId",
                table: "Anthologies",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Anthologies_Users_AuthorId",
                table: "Anthologies",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
