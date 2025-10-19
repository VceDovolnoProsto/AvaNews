using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvaNews.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "news",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider_id = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    summary = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: false),
                    published_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    publisher_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    publisher_logo_url = table.Column<string>(type: "text", nullable: true),
                    publisher_homepage_url = table.Column<string>(type: "text", nullable: true),
                    tickers = table.Column<string[]>(type: "text[]", nullable: true),
                    keywords = table.Column<string[]>(type: "text[]", nullable: true),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    enr_price_now = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    enr_prev_close = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    enr_change_pct = table.Column<double>(type: "double precision", nullable: true),
                    enr_sentiment = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_news", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    tickers = table.Column<string>(type: "jsonb", nullable: true),
                    query_text = table.Column<string>(type: "text", nullable: true),
                    channel = table.Column<string>(type: "text", nullable: false),
                    target = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_news_provider_id",
                table: "news",
                column: "provider_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_news_published_desc",
                table: "news",
                column: "published_utc");

            migrationBuilder.CreateIndex(
                name: "IX_news_tickers",
                table: "news",
                column: "tickers")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_user_id",
                table: "subscriptions",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "news");

            migrationBuilder.DropTable(
                name: "subscriptions");
        }
    }
}
