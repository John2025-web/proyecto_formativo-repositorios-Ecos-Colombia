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
                modelBuilder.Entity<culturaTradicion>().ToTable("CULTURA_TRADICION")
                    .HasKey(link => new { link.idCultura, link.idTradicion });
                modelBuilder.Entity<culturaTradicion>().Property(link => link.idCultura)
                    .HasColumnName("id_cultura");
                modelBuilder.Entity<culturaTradicion>().Property(link => link.idTradicion)
                    .HasColumnName("id_tradicion");
            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }

        }
}