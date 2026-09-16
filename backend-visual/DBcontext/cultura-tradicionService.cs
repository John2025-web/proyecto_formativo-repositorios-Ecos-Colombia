using Microsoft.EntityFrameworkCore;
using ApiPrueba.models;

namespace ApiPrueba.DBcontex
{
   
        public class cultura_TradicionService : DbContext
        {
            public cultura_TradicionService(DbContextOptions options) : base(options)
            {
            }
            public DbSet<culturaTradicion> culturaTradicion { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                EntityConfiguration(modelBuilder);
            }
            private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<culturaTradicion>().ToTable("cultura").HasKey(u => u.idCultura);
                modelBuilder.Entity<culturaTradicion>().ToTable("IDIOMA").HasKey(u => u.idTradicion);
            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }

        }
}