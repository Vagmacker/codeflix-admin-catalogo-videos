using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FC.Codeflix.Catalog.Infra.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class CreateCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(name: "id", type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(name: "name", type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Active = table.Column<bool>(name: "is_active", type: "tinyint(1)", nullable: false),
                    Description = table.Column<string>(name: "description", type: "varchar(4000)", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(name: "created_at", type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(name: "updated_at", type: "datetime(6)", nullable: false),
                    DeletedAt = table.Column<DateTime>(name: "deleted_at", type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "categories");
        }
    }
}
