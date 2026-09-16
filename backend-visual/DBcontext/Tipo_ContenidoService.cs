using Microsoft.EntityFrameworkCore;
using ApiPrueba.Models;

namespace ApiPrueba.DBcontext
{
    public class Tipo_ContenidoService : DbContext
    {
        public Tipo_ContenidoService(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Tipo_Contenido> tipo_Contenido { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }
        private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Tipo_Contenido>().ToTable("idtipo").HasKey(u => u.idTipo);

                modelBuilder.Entity<Tipo_Contenido>().Property(u => u.Nombre).HasColumnName("Nombre");

                modelBuilder.Entity<Tipo_Contenido>().Property(t => t.Descripcion).HasColumnName("Descripcion").HasMaxLength(250);
            }
        public async Task<bool> SaveAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }

    }
}
