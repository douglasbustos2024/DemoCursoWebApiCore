using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppApiArq.Migrations
{
    public partial class TablasRelacionadasAProducto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "tbProducts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "tbProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tbCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbSuppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSuppliers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbProducts_CategoryId",
                table: "tbProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tbProducts_SupplierId",
                table: "tbProducts",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbProducts_tbCategories_CategoryId",
                table: "tbProducts",
                column: "CategoryId",
                principalTable: "tbCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbProducts_tbSuppliers_SupplierId",
                table: "tbProducts",
                column: "SupplierId",
                principalTable: "tbSuppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbProducts_tbCategories_CategoryId",
                table: "tbProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_tbProducts_tbSuppliers_SupplierId",
                table: "tbProducts");

            migrationBuilder.DropTable(
                name: "tbCategories");

            migrationBuilder.DropTable(
                name: "tbSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_tbProducts_CategoryId",
                table: "tbProducts");

            migrationBuilder.DropIndex(
                name: "IX_tbProducts_SupplierId",
                table: "tbProducts");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "tbProducts");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "tbProducts");
        }
    }
}
