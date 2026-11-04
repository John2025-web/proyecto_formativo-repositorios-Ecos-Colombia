using ApiPrueba.models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ApiPrueba.Data
{
    public class DatabaseServiceCONTENIDO_CULTURA : DbContext
    {
        public DatabaseServiceCONTENIDO_CULTURA(
         DbContextOptions<DatabaseServiceCONTENIDO_CULTURA> options
         ) : base(options)
        {
        }


        public DbSet<CONTENIDO_CULTURA> CONTENIDO_CULTURA { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }

        private void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CONTENIDO_CULTURA>().ToTable("CONTENIDO_CULTURA");

            modelBuilder.Entity<CONTENIDO_CULTURA>()
                .HasKey(u => new { u.IdContenido, u.IdCultura });

            modelBuilder.Entity<CONTENIDO_CULTURA>()
                .Property(u => u.IdContenido)
                .HasColumnName("id_contenido");

            modelBuilder.Entity<CONTENIDO_CULTURA>()
                .Property(u => u.IdCultura)
                .HasColumnName("id_cultura");
        }

        public async Task<bool> SaveAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}