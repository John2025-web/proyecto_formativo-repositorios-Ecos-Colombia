using ApiPrueba.models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ApiPrueba.Data
{
    public class DatabaseServiceCONTENIDO_LUGAR : DbContext
    {
        public DatabaseServiceCONTENIDO_LUGAR(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CONTENIDO_LUGAR> CONTENIDO_LUGAR { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }

        private void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CONTENIDO_LUGAR>().ToTable("CONTENIDO_LUGAR");

            modelBuilder.Entity<CONTENIDO_LUGAR>()
                .HasKey(u => new { u.IdContenido, u.IdLugar });
            modelBuilder.Entity<CONTENIDO_LUGAR>()
                .Property(u => u.IdContenido)
                .HasColumnName("id_contenido");
            modelBuilder.Entity<CONTENIDO_LUGAR>()
                .Property(u => u.IdLugar)
                .HasColumnName("id_lugar");
        }

        public async Task<bool> SaveAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
