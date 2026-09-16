using ApiPrueba.models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ApiPrueba.Data
{
    public class DatabaseServiceCONTENIDO : DbContext
    {
      
        public DatabaseServiceCONTENIDO(DbContextOptions<DatabaseServiceCONTENIDO> options)
            : base(options)
        {
        }

        public DbSet<CONTENIDO> CONTENIDO { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }

        private void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CONTENIDO>().ToTable("CONTENIDO");
            modelBuilder.Entity<CONTENIDO>().HasKey(u => u.IdContenido);
            modelBuilder.Entity<CONTENIDO>().Property(u => u.IdContenido).HasColumnName("id_contenido").ValueGeneratedOnAdd();
            modelBuilder.Entity<CONTENIDO>().Property(u => u.Titulo).HasColumnName("titulo");
            modelBuilder.Entity<CONTENIDO>().Property(u => u.Descripcion).HasColumnName("descripcion");
            modelBuilder.Entity<CONTENIDO>().Property(u => u.IdTipo).HasColumnName("Id_Tipo");
            modelBuilder.Entity<CONTENIDO>().Property(u => u.FechaPublicacion) .HasColumnName("fecha_publicacion");
        }

        public async Task<bool> SaveAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
