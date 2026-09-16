using Microsoft.EntityFrameworkCore;
using ApiPrueba.Models;

namespace ApiPrueba.DBcontext
{
    public class RegionService : DbContext
    {
        public RegionService(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Region> RegionServices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {       
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }
        private void EntityConfiguration(ModelBuilder modelBuilder)
        {
                modelBuilder.Entity<Region>().ToTable("Region").HasKey(r => r.idRegion);

                modelBuilder.Entity<Region>().Property(r => r.Nombre).HasColumnName("Nombre").HasMaxLength(100).IsRequired();

                modelBuilder.Entity<Region>().HasMany(r => r.Tradiciones).WithOne(t => t.Region).HasForeignKey(t => t.idRegion).OnDelete(DeleteBehavior.Restrict);
            
        }
        public async Task<bool> SaveAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}

