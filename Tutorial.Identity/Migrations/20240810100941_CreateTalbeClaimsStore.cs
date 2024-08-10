using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tutorial.Identity.Migrations
{
    /// <inheritdoc />
    public partial class CreateTalbeClaimsStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClaimsStores",
                columns: table => new
                {
                    ClaimsStoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimsStores", x => x.ClaimsStoreId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimsStores");
        }
    }
}
