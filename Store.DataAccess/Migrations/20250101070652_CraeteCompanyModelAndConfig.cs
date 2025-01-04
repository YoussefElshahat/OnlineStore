using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CraeteCompanyModelAndConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Entity_Id_NotZero1",
                table: "products");

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(256)", maxLength: 256, nullable: false),
                    StreetAdress = table.Column<string>(type: "VARCHAR(256)", maxLength: 256, nullable: false),
                    City = table.Column<string>(type: "VARCHAR(256)", maxLength: 256, nullable: false),
                    State = table.Column<string>(type: "VARCHAR(256)", maxLength: 256, nullable: false),
                    PostalCode = table.Column<string>(type: "VARCHAR(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "VARCHAR(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.CheckConstraint("CK_Entity_Id_NotZero1", "[Id] <> 0");
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Entity_Id_NotZero2",
                table: "products",
                sql: "[Id] <> 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Entity_Id_NotZero2",
                table: "products");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Entity_Id_NotZero1",
                table: "products",
                sql: "[Id] <> 0");
        }
    }
}
