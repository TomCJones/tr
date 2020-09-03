using Microsoft.EntityFrameworkCore.Migrations;

namespace tr.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sub = table.Column<string>(nullable: true),
                    method = table.Column<int>(nullable: false),
                    Created = table.Column<long>(nullable: false),
                    Updated = table.Column<long>(nullable: false),
                    Expiry = table.Column<long>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    Status = table.Column<string>(maxLength: 20, nullable: true),
                    Purpose = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
