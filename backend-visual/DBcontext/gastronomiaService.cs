using Microsoft.EntityFrameworkCore;
using ApiPrueba.models;
using ApiPrueba.Repositorios.Interfaces;

namespace ApiPrueba .DBcontex
{
    public class interfaceGastroRepo
    {
        public class GastronomiaService : DbContext
        {
            public GastronomiaService(DbContextOptions options) : base(options)
            {
            }
            public DbSet<GASTRONOMIA> Gastronomia { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                EntityConfiguration(modelBuilder);
            }
            private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<GASTRONOMIA>().ToTable("IdPlato").HasKey(e => e.IdPlato);
                modelBuilder.Entity<GASTRONOMIA>().ToTable("Idregion").HasKey(e => e.IdRegion);
                modelBuilder.Entity<GASTRONOMIA>().Property(u => u.Nombre).HasColumnName("Nombre").IsRequired();
                modelBuilder.Entity<GASTRONOMIA>().Property(u => u.Ingredientes).HasColumnName("Ingredientes").IsRequired();
                modelBuilder.Entity<GASTRONOMIA>().Property(u => u.DescripcionPlato).HasColumnName("descripcion").IsRequired();
                modelBuilder.Entity<GASTRONOMIA>().Property(u => u.ImagenPlato).HasColumnName("imagen").IsRequired();
            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }

        }
    }
}