using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VerseCraft.Migrations
{
    /// <inheritdoc />
    public partial class FixingSomeShits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Anthologies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AuthorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LicenseType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CopyrightNotice = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsApproved = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anthologies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Anthologies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OTPTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Token = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Expiry = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OTPTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Poems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Genre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Style = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Theme = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Language = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Mood = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LicenseType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CopyrightNotice = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CoverImagePath = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    AuthorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsApproved = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Poems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavedAnthologies",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AnthologyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    DateSaved = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedAnthologies", x => new { x.UserId, x.AnthologyId });
                    table.ForeignKey(
                        name: "FK_SavedAnthologies_Anthologies_AnthologyId",
                        column: x => x.AnthologyId,
                        principalTable: "Anthologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedAnthologies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "ApprovedItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkType = table.Column<int>(type: "INTEGER", maxLength: 50, nullable: false),
                    PoemId = table.Column<int>(type: "INTEGER", nullable: true),
                    AnthologyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovedItems_Anthologies_AnthologyId",
                        column: x => x.AnthologyId,
                        principalTable: "Anthologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ApprovedItems_Poems_PoemId",
                        column: x => x.PoemId,
                        principalTable: "Poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FeaturedItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkType = table.Column<int>(type: "INTEGER", maxLength: 50, nullable: false),
                    PoemId = table.Column<int>(type: "INTEGER", nullable: true),
                    AnthologyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeaturedItems_Anthologies_AnthologyId",
                        column: x => x.AnthologyId,
                        principalTable: "Anthologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FeaturedItems_Poems_PoemId",
                        column: x => x.PoemId,
                        principalTable: "Poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PublicItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkType = table.Column<int>(type: "INTEGER", maxLength: 50, nullable: false),
                    PoemId = table.Column<int>(type: "INTEGER", nullable: true),
                    AnthologyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicItems_Anthologies_AnthologyId",
                        column: x => x.AnthologyId,
                        principalTable: "Anthologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PublicItems_Poems_PoemId",
                        column: x => x.PoemId,
                        principalTable: "Poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SavedPoems",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PoemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    DateSaved = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedPoems", x => new { x.UserId, x.PoemId });
                    table.ForeignKey(
                        name: "FK_SavedPoems_Poems_PoemId",
                        column: x => x.PoemId,
                        principalTable: "Poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedPoems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anthologies_UserId",
                table: "Anthologies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnthologyPoems_PoemId",
                table: "AnthologyPoems",
                column: "PoemId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovedItems_AnthologyId",
                table: "ApprovedItems",
                column: "AnthologyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovedItems_PoemId",
                table: "ApprovedItems",
                column: "PoemId");

            migrationBuilder.CreateIndex(
                name: "IX_FeaturedItems_AnthologyId",
                table: "FeaturedItems",
                column: "AnthologyId");

            migrationBuilder.CreateIndex(
                name: "IX_FeaturedItems_PoemId",
                table: "FeaturedItems",
                column: "PoemId");

            migrationBuilder.CreateIndex(
                name: "IX_OTPTokens_UserId",
                table: "OTPTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Poems_UserId",
                table: "Poems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicItems_AnthologyId",
                table: "PublicItems",
                column: "AnthologyId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicItems_PoemId",
                table: "PublicItems",
                column: "PoemId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedAnthologies_AnthologyId",
                table: "SavedAnthologies",
                column: "AnthologyId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedPoems_PoemId",
                table: "SavedPoems",
                column: "PoemId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnthologyPoems");

            migrationBuilder.DropTable(
                name: "ApprovedItems");

            migrationBuilder.DropTable(
                name: "FeaturedItems");

            migrationBuilder.DropTable(
                name: "OTPTokens");

            migrationBuilder.DropTable(
                name: "PublicItems");

            migrationBuilder.DropTable(
                name: "SavedAnthologies");

            migrationBuilder.DropTable(
                name: "SavedPoems");

            migrationBuilder.DropTable(
                name: "Anthologies");

            migrationBuilder.DropTable(
                name: "Poems");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
