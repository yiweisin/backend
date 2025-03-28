using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class Ini : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stocks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    symbol = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stocks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: false),
                    passwordhash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tradehistories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stockid = table.Column<int>(type: "integer", nullable: false),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    entryprice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    pnl = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isholding = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tradehistories", x => x.id);
                    table.ForeignKey(
                        name: "FK_tradehistories_stocks_stockid",
                        column: x => x.stockid,
                        principalTable: "stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tradehistories_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "stocks",
                columns: new[] { "id", "description", "name", "price", "symbol" },
                values: new object[,]
                {
                    { 1, "Technology company that designs, manufactures, and markets smartphones, personal computers, tablets, wearables, and accessories.", "Apple Inc.", 187.41m, "AAPL" },
                    { 2, "Technology company that develops, licenses, and supports software, services, devices, and solutions.", "Microsoft Corporation", 403.78m, "MSFT" },
                    { 3, "Online retailer and web service provider.", "Amazon.com, Inc.", 178.75m, "AMZN" },
                    { 4, "Technology company that specializes in Internet-related services and products.", "Alphabet Inc.", 153.51m, "GOOGL" },
                    { 5, "Technology company that focuses on social media and technology.", "Meta Platforms, Inc.", 485.58m, "META" },
                    { 6, "Electric vehicle and clean energy company.", "Tesla, Inc.", 175.34m, "TSLA" },
                    { 7, "Technology company that designs graphics processing units.", "NVIDIA Corporation", 952.76m, "NVDA" },
                    { 8, "Financial services company.", "JPMorgan Chase & Co.", 198.47m, "JPM" },
                    { 9, "Digital payments company.", "Visa Inc.", 267.32m, "V" },
                    { 10, "Pharmaceutical and medical devices company.", "Johnson & Johnson", 153.92m, "JNJ" },
                    { 11, "Retail corporation that operates a chain of hypermarkets.", "Walmart Inc.", 65.32m, "WMT" },
                    { 12, "Consumer goods corporation.", "Procter & Gamble Co.", 161.43m, "PG" },
                    { 13, "Multinational investment bank and financial services company.", "Bank of America Corp.", 39.25m, "BAC" },
                    { 14, "Health insurance company.", "UnitedHealth Group Inc.", 528.72m, "UNH" },
                    { 15, "Home improvement retailer.", "Home Depot Inc.", 343.17m, "HD" },
                    { 16, "Multinational oil and gas corporation.", "Exxon Mobil Corp.", 115.44m, "XOM" },
                    { 17, "Pharmaceutical corporation.", "Pfizer Inc.", 27.56m, "PFE" },
                    { 18, "Designer, developer, and global supplier of semiconductor devices.", "Broadcom Inc.", 1256.83m, "AVGO" },
                    { 19, "Technology company that develops networking hardware and software.", "Cisco Systems, Inc.", 48.90m, "CSCO" },
                    { 20, "Telecommunications conglomerate.", "Verizon Communications Inc.", 41.75m, "VZ" },
                    { 21, "Chain of membership-only warehouse clubs.", "Costco Wholesale Corp.", 721.24m, "COST" },
                    { 22, "Multinational energy corporation.", "Chevron Corp.", 146.27m, "CVX" },
                    { 23, "Multinational computer software company.", "Adobe Inc.", 525.31m, "ADBE" },
                    { 24, "Telecommunications conglomerate.", "Comcast Corp.", 42.15m, "CMCSA" },
                    { 25, "Athletic footwear and apparel corporation.", "Nike, Inc.", 81.24m, "NKE" },
                    { 26, "Streaming service and production company.", "Netflix, Inc.", 678.69m, "NFLX" },
                    { 27, "Food, snack, and beverage corporation.", "PepsiCo, Inc.", 174.81m, "PEP" },
                    { 28, "Beverage corporation.", "Coca-Cola Co.", 60.37m, "KO" },
                    { 29, "Medical devices and health care company.", "Abbott Laboratories", 108.25m, "ABT" },
                    { 30, "Pharmaceutical company.", "Merck & Co., Inc.", 121.45m, "MRK" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "passwordhash", "username" },
                values: new object[] { 1, "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8", "demo" });

            migrationBuilder.InsertData(
                table: "tradehistories",
                columns: new[] { "id", "date", "entryprice", "isholding", "pnl", "stockid", "userid" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 26, 0, 0, 0, 0, DateTimeKind.Utc), 150.25m, true, 37.16m, 1, 1 },
                    { 2, new DateTime(2025, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc), 380.50m, true, 23.28m, 2, 1 },
                    { 3, new DateTime(2025, 3, 18, 0, 0, 0, 0, DateTimeKind.Utc), 185.30m, false, -6.55m, 3, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tradehistories_stockid",
                table: "tradehistories",
                column: "stockid");

            migrationBuilder.CreateIndex(
                name: "IX_tradehistories_userid",
                table: "tradehistories",
                column: "userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tradehistories");

            migrationBuilder.DropTable(
                name: "stocks");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
