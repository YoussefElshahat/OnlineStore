﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Models;

namespace JOStore.Data.DataConfig
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
              .ValueGeneratedNever();


            builder.Property(x => x.Name)
                .HasColumnType("VARCHAR")
                .IsRequired().HasMaxLength(255);
                 

            builder.ToTable("Categories",
                t => t.HasCheckConstraint
                ("CK_Entity_Id_NotZero", "[Id] <> 0")); // Enforce that Id cannot be 0.
            

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
