using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Data.Config
{
    internal class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        

        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(x => x.Name).HasColumnType("VARCHAR").HasMaxLength(256).IsRequired();
              
            builder.Property(x => x.StreetAddress).HasColumnType("VARCHAR").HasMaxLength(256);
                
            builder.Property(x => x.Region).HasColumnType("VARCHAR").HasMaxLength(256);
            builder.Property(x => x.PostalCode).HasColumnType("VARCHAR").HasMaxLength(256);
            
        }
    }
}
