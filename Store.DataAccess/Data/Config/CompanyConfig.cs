using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Models;

namespace Store.DataAccess.Data.Config
{
    internal class CompanyConfig : IEntityTypeConfiguration<Company>
    {
    
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd()
                .HasAnnotation("SqlServer:ValueGenerationStrategy"
                , SqlServerValueGenerationStrategy.IdentityColumn);

            builder.Property(x =>x.Name).IsRequired()
                .HasColumnType("VARCHAR").HasMaxLength(256);

            builder.Property(x => x.StreetAdress)
                .HasColumnType("VARCHAR").HasMaxLength(256);
            builder.Property(x => x.City)
                .HasColumnType("VARCHAR").HasMaxLength(256);
            builder.Property(x => x.State).IsRequired()
                .HasColumnType("VARCHAR").HasMaxLength(256);
            builder.Property(x => x.PhoneNumber)
                .HasColumnType("VARCHAR").HasMaxLength(256);
            builder.Property(x => x.PostalCode)
                .HasColumnType("VARCHAR").HasMaxLength(256);


            builder.ToTable("Companies",
               t => t.HasCheckConstraint
               ("CK_Entity_Id_NotZero", "[Id] <> 0")); // Enforce that Id can
            
        }
    }
}
