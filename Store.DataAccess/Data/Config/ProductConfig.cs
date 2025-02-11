using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Models;

namespace Store.DataAccess.Data.Config
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);


            builder.Property(x => x.Name).IsRequired()
                .HasColumnType("VARCHAR(MAX)");


            builder.Property(x => x.Price).IsRequired().HasColumnType("decimal");

            builder.Property(X => X.Description)
                .IsRequired().HasColumnType("VARCHAR(MAX)");


            builder.HasOne(x => x.Category)
            .WithMany(c => c.Products)  // Assuming 'Products' is a collection of related entities
            .HasForeignKey(x => x.CategoryId)  // Foreign key property
            .IsRequired();

           
            builder.ToTable("products",
               t => t.HasCheckConstraint
               ("CK_Entity_Id_NotZero", "[Id] <> 0")); // Enforce that Id cannot be 0.

            builder.HasData(LoadProducts());
        }
        private static List<Product> LoadProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = -1,
                    Name = "MSI A520M-A Pro Motherboard AM4",
                    Price = 2950.00m,
                    Description ="Brand: MSI, Model: A520M-A PRO, CPU Support: AMD Ryzen 5000 Series, 5000 G-Series, 4000 G-Series, 3000 Series and 3000 G-Series Processors, CPU Socket: AMD AM4, Chipset: AMD A520, Graphics Interface: 1x PCI-E 3.0 x16 Slot, Display Interface: HDMI, DVI-D – Requires Processor Graphics, Memory Support: 2 DIMMs, Dual Channel DDR4-4600MHz (OC)",
                    CategoryId = 1,
                },
                new Product
                {
                    Id= -2,
                    Name = "AMD Ryzen 7 9800X3D Processor (5.2GHz/104MB) 8 Core AM5",
                    Price = 27999.00m,
                    Description = "Brand:AMD, Type: Processor, Processor Family: Ryzen 7, Processor Generation: 9800X3D, Socket Type: AM5, CPU Core: 8, Base Clock: 4.7GHz, Max Boost Clock: Up to 5.2GHz, Total L1 Cache: 640KB, Total L2 Cache: 8MB, Total L3 Cache: 96MB, Processor Graphics: Radeon Graphics 2200 MHz, Max Temperature: 95°C, Warranty: 3 Years ",
                    CategoryId = 2,

                },
                new Product
                {
                    Id= -3,
                    Name = "Zotac Gaming GeForce RTX 4060 Ti Twin Edge 16GB GDDR6 Graphics Card",
                    Price = 23500.00m,
                    Description = "Brand:Zotac Model: GeForce RTX 4060 Ti Twin Edge Type:GraphicsCardCoreClock: Boost:2535MHz,CUDA Cores: 4352,MemoryClock: 18 Gbps, Memory: 16GB - 128-bit - GDDR6",
                    CategoryId = 3,
                }
             };
        }



    }
}
