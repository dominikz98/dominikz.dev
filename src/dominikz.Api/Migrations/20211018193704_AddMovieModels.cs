using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dominikz.Api.Migrations
{
    public partial class AddMovieModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "movie_ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Actors = table.Column<int>(type: "INTEGER", maxLength: 5, nullable: false),
                    Ambience = table.Column<int>(type: "INTEGER", maxLength: 5, nullable: false),
                    Music = table.Column<int>(type: "INTEGER", maxLength: 5, nullable: false),
                    Plot = table.Column<int>(type: "INTEGER", maxLength: 5, nullable: false),
                    Regie = table.Column<int>(type: "INTEGER", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_ratings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "movie_starts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Surename = table.Column<string>(type: "TEXT", nullable: false),
                    Job = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_starts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KeyWord = table.Column<string>(type: "TEXT", nullable: false),
                    MDContent = table.Column<string>(type: "TEXT", nullable: false),
                    Thumbnail = table.Column<string>(type: "TEXT", nullable: false),
                    YoutubeTrailerId = table.Column<string>(type: "TEXT", nullable: true),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false),
                    Runtime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Publication = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Watched = table.Column<DateTime>(type: "TEXT", nullable: false),
                    USK = table.Column<int>(type: "INTEGER", nullable: false),
                    RatingId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_movies_activities_Id",
                        column: x => x.Id,
                        principalTable: "activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movies_movie_ratings_RatingId",
                        column: x => x.RatingId,
                        principalTable: "movie_ratings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieMovieStar",
                columns: table => new
                {
                    MoviesId = table.Column<int>(type: "INTEGER", nullable: false),
                    StarsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieMovieStar", x => new { x.MoviesId, x.StarsId });
                    table.ForeignKey(
                        name: "FK_MovieMovieStar_movie_starts_StarsId",
                        column: x => x.StarsId,
                        principalTable: "movie_starts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieMovieStar_movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieMovieStar_StarsId",
                table: "MovieMovieStar",
                column: "StarsId");

            migrationBuilder.CreateIndex(
                name: "IX_movies_RatingId",
                table: "movies",
                column: "RatingId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieMovieStar");

            migrationBuilder.DropTable(
                name: "movie_starts");

            migrationBuilder.DropTable(
                name: "movies");

            migrationBuilder.DropTable(
                name: "movie_ratings");
        }
    }
}
