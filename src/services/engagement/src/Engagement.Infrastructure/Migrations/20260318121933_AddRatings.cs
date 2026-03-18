using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Engagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ratings",
                schema: "engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ratings_BookId",
                schema: "engagement",
                table: "ratings",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_BookId_UserId",
                schema: "engagement",
                table: "ratings",
                columns: new[] { "BookId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ratings",
                schema: "engagement");
        }
    }
}
