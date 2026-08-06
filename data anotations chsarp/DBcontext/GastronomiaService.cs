using Microsoft.EntityFrameworkCore;
using pruebaApi.models;

namespace pruebaApi.DBcontext
{
    public class GastronomiaService
    {
        public class DatabaseService : DbContext
        {
            public DatabaseService(DbContextOptions options) : base(options)
            {
            }

            public DbSet<Gastronomia> Gastronomia { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                EntityConfiguration(modelBuilder);
            }

            private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Gastronomia>().ToTable("Gastronomia");
                modelBuilder.Entity<Gastronomia>().HasKey(u => u.IdPlato);
                modelBuilder.Entity<Gastronomia>().Property(u => u.Nombre).HasColumnName("Nombre").ValueGeneratedOnAdd();
                modelBuilder.Entity<Gastronomia>().Property(u => u.IdRegion).HasColumnName("").ValueGeneratedOnAdd();
                modelBuilder.Entity<Gastronomia>().Property(u => u.DescripcionPlato).HasColumnName("DescripcionPlato").ValueGeneratedOnAdd();
                modelBuilder.Entity<Gastronomia>().Property(u => u.ImagenPlato).HasColumnName("ImagenPlato").ValueGeneratedOnAdd();
            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }
        }
    }
}
