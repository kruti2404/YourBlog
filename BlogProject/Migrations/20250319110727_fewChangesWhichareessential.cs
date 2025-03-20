using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogProject.Migrations
{
    /// <inheritdoc />
    public partial class fewChangesWhichareessential : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogsGenres_Blogs_BlogsId",
                table: "BlogsGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogsGenres_Genres_GenresId",
                table: "BlogsGenres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogsGenres",
                table: "BlogsGenres");

            migrationBuilder.RenameTable(
                name: "BlogsGenres",
                newName: "BlogsGenre");

            migrationBuilder.RenameIndex(
                name: "IX_BlogsGenres_GenresId",
                table: "BlogsGenre",
                newName: "IX_BlogsGenre_GenresId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogsGenre",
                table: "BlogsGenre",
                columns: new[] { "BlogsId", "GenresId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BlogsGenre_Blogs_BlogsId",
                table: "BlogsGenre",
                column: "BlogsId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogsGenre_Genres_GenresId",
                table: "BlogsGenre",
                column: "GenresId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogsGenre_Blogs_BlogsId",
                table: "BlogsGenre");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogsGenre_Genres_GenresId",
                table: "BlogsGenre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogsGenre",
                table: "BlogsGenre");

            migrationBuilder.RenameTable(
                name: "BlogsGenre",
                newName: "BlogsGenres");

            migrationBuilder.RenameIndex(
                name: "IX_BlogsGenre_GenresId",
                table: "BlogsGenres",
                newName: "IX_BlogsGenres_GenresId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogsGenres",
                table: "BlogsGenres",
                columns: new[] { "BlogsId", "GenresId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BlogsGenres_Blogs_BlogsId",
                table: "BlogsGenres",
                column: "BlogsId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogsGenres_Genres_GenresId",
                table: "BlogsGenres",
                column: "GenresId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
