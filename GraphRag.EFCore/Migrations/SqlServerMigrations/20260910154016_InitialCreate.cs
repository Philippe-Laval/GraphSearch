using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphRag.EFCore.Migrations.SqlServerMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GraphEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GraphEntityAlias",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphEntityAlias", x => new { x.EntityId, x.Alias });
                    table.ForeignKey(
                        name: "FK_GraphEntityAlias_GraphEntity_EntityId",
                        column: x => x.EntityId,
                        principalTable: "GraphEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GraphEntity_Name",
                table: "GraphEntity",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_GraphEntity_Type",
                table: "GraphEntity",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_GraphEntityAlias_Alias",
                table: "GraphEntityAlias",
                column: "Alias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GraphEntityAlias");

            migrationBuilder.DropTable(
                name: "GraphEntity");
        }
    }
}
