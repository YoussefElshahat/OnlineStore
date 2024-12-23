using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JOStore.Migrations
{
    /// <inheritdoc />
    public partial class AddProductToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(MAX)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(MAX)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                    table.CheckConstraint("CK_Entity_Id_NotZero1", "[Id] <> 0");
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "Id", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Brand: MSI, Model: A520M-A PRO, CPU Support: AMD Ryzen 5000 Series, 5000 G-Series, 4000 G-Series, 3000 Series and 3000 G-Series Processors, CPU Socket: AMD AM4, Chipset: AMD A520, Graphics Interface: 1x PCI-E 3.0 x16 Slot, Display Interface: HDMI, DVI-D – Requires Processor Graphics, Memory Support: 2 DIMMs, Dual Channel DDR4-4600MHz (OC)", "MSI A520M-A Pro Motherboard AM4", 2950.00m },
                    { 2, "Brand:AMD, Type: Processor, Processor Family: Ryzen 7, Processor Generation: 9800X3D, Socket Type: AM5, CPU Core: 8, Base Clock: 4.7GHz, Max Boost Clock: Up to 5.2GHz, Total L1 Cache: 640KB, Total L2 Cache: 8MB, Total L3 Cache: 96MB, Processor Graphics: Radeon Graphics 2200 MHz, Max Temperature: 95°C, Warranty: 3 Years ", "AMD Ryzen 7 9800X3D Processor (5.2GHz/104MB) 8 Core AM5", 27999.00m },
                    { 3, "Brand:Zotac Model: GeForce RTX 4060 Ti Twin Edge Type:GraphicsCardCoreClock: Boost:2535MHz,CUDA Cores: 4352,MemoryClock: 18 Gbps, Memory: 16GB - 128-bit - GDDR6", "Zotac Gaming GeForce RTX 4060 Ti Twin Edge 16GB GDDR6 Graphics Card", 23500.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
