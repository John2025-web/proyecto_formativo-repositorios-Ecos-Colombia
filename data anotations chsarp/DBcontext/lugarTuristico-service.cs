using Microsoft.EntityFrameworkCore;
using pruebaApi.models;

namespace pruebaApi.DBcontext
{
    public class LugarTuristicoservice
    {
        public class DatabaseService : DbContext
        {
            public DatabaseService(DbContextOptions options) : base(options)
            {
            }

            public DbSet<LugarTuristico> LugarTuristico { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                EntityConfiguration(modelBuilder);
            }

            private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<LugarTuristico>().ToTable("lugar turistico");
                modelBuilder.Entity<LugarTuristico>().HasKey(u => u.IdLugar);
                modelBuilder.Entity<LugarTuristico>().Property(u => u.Nombre).HasColumnName("Nombre").ValueGeneratedOnAdd();
                modelBuilder.Entity<LugarTuristico>().Property(u => u.Ciudad).HasColumnName("Ciudad").ValueGeneratedOnAdd();
                modelBuilder.Entity<LugarTuristico>().Property(u => u.Descripcion).HasColumnName("descripcion").ValueGeneratedOnAdd();
                modelBuilder.Entity<LugarTuristico>().Property(u => u.Imagen).HasColumnName("Nombre").ValueGeneratedOnAdd();
                modelBuilder.Entity<LugarTuristico>().Property(u => u.ubicacion).HasColumnName("ubicacion").ValueGeneratedOnAdd();

            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }

        }
    }
}