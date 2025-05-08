using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VerseCraft.Migrations
{
    /// <inheritdoc />
    public partial class KlarohonTiniYawa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Anthologies");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Anthologies");

            migrationBuilder.DropColumn(
                name: "TermsAndConditions",
                table: "Anthologies");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Poems",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "AnthologyId",
                table: "Poems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Poems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CoverImagePath",
                table: "Poems",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Poems",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Poems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Poems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Poems",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mood",
                table: "Poems",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Style",
                table: "Poems",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Poems",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Poems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseType",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "CopyrightNotice",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Anthologies",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Anthologies",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Anthologies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Poems_AnthologyId",
                table: "Poems",
                column: "AnthologyId");

            migrationBuilder.CreateIndex(
                name: "IX_Anthologies_UserId",
                table: "Anthologies",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Anthologies_Users_UserId",
                table: "Anthologies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Poems_Anthologies_AnthologyId",
                table: "Poems",
                column: "AnthologyId",
                principalTable: "Anthologies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Anthologies_Users_UserId",
                table: "Anthologies");

            migrationBuilder.DropForeignKey(
                name: "FK_Poems_Anthologies_AnthologyId",
                table: "Poems");

            migrationBuilder.DropIndex(
                name: "IX_Poems_AnthologyId",
                table: "Poems");

            migrationBuilder.DropIndex(
                name: "IX_Anthologies_UserId",
                table: "Anthologies");

            migrationBuilder.DropColumn(
                name: "AnthologyId",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "CoverImagePath",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "Mood",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "Style",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Poems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Anthologies");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Anthologies");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Anthologies");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Anthologies");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Anthologies");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Poems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseType",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CopyrightNotice",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Anthologies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TermsAndConditions",
                table: "Anthologies",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PoemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Poems_PoemId",
                        column: x => x.PoemId,
                        principalTable: "Poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PoemId",
                table: "Comments",
                column: "PoemId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");
        }
    }
}
