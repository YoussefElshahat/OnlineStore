using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace JOStore.Data
{
    public class AppDbContext :IdentityDbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options) 
        {

                
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

    }
}
