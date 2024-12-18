using JOStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JOStore.Data.DataConfig
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x =>x.Id )
                .IsRequired().ValueGeneratedNever();

            builder.Property(x => x.Name)
                .HasColumnType("VARCHAR")
                .HasMaxLength(255)
                .IsRequired();

            builder.ToTable("Categories");

            builder.HasData(LoadCategories());
                
        }

        private static List<Category> LoadCategories()
        {
            return new List<Category>
            {
                new Category { Id = 1,Name = "MOTHERBOARD" },
                new Category { Id = 2,Name = "PROCESSOR" },
                new Category { Id = 3,Name = "GRAPHICS CARD" }
            };
        }
    }
}
