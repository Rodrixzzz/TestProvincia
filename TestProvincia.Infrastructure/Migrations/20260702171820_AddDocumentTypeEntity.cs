using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TestProvincia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentTypeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DocumentType",
                table: "Users",
                newName: "DocumentTypeId");

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Desc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DocumentTypes",
                columns: new[] { "Id", "Active", "Desc" },
                values: new object[,]
                {
                    { 1, true, "DNI" },
                    { 2, true, "Pasaporte" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_DocumentNumber_DocumentTypeId",
                table: "Users",
                columns: new[] { "DocumentNumber", "DocumentTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DocumentTypeId",
                table: "Users",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_DocumentTypes_DocumentTypeId",
                table: "Users",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_DocumentTypes_DocumentTypeId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Users_DocumentNumber_DocumentTypeId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DocumentTypeId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "DocumentTypeId",
                table: "Users",
                newName: "DocumentType");
        }
    }
}
