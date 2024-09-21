using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppApiArq.Migrations
{
    public partial class AgregarKardex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbBalances_tbProducts_ProductId",
                table: "tbBalances");

            migrationBuilder.DropForeignKey(
                name: "FK_tbBalances_tbUser_UserId",
                table: "tbBalances");

            migrationBuilder.CreateTable(
                name: "tbKardexs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TipoId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbKardexs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbKardexs_tbProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "tbProducts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tbKardexs_tbUser_UserId",
                        column: x => x.UserId,
                        principalTable: "tbUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbKardexs_ProductId",
                table: "tbKardexs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_tbKardexs_UserId",
                table: "tbKardexs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbBalances_tbProducts_ProductId",
                table: "tbBalances",
                column: "ProductId",
                principalTable: "tbProducts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbBalances_tbUser_UserId",
                table: "tbBalances",
                column: "UserId",
                principalTable: "tbUser",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbBalances_tbProducts_ProductId",
                table: "tbBalances");

            migrationBuilder.DropForeignKey(
                name: "FK_tbBalances_tbUser_UserId",
                table: "tbBalances");

            migrationBuilder.DropTable(
                name: "tbKardexs");

            migrationBuilder.AddForeignKey(
                name: "FK_tbBalances_tbProducts_ProductId",
                table: "tbBalances",
                column: "ProductId",
                principalTable: "tbProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbBalances_tbUser_UserId",
                table: "tbBalances",
                column: "UserId",
                principalTable: "tbUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
