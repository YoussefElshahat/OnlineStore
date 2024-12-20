using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOStore.Migrations
{
    /// <inheritdoc />
    public partial class CK_Entity_Id_NotZero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Entity_Id_NotZero",
                table: "Categories",
                sql: "[Id] <> 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Entity_Id_NotZero",
                table: "Categories");
        }
    }
}
