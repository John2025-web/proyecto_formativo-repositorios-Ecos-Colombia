using Microsoft.EntityFrameworkCore;
using ApiPrueba.models;

namespace ApiPrueba.DBcontex
{

        public class lugar_turistico_Service : DbContext
        {
        public lugar_turistico_Service(
            DbContextOptions<lugar_turistico_Service> options
        ) : base(options)
        {
        }
        public DbSet<LUGAR_TURISTICO> lugar_Turistico { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                EntityConfiguration(modelBuilder);
            }
            private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<LUGAR_TURISTICO>().ToTable("idLugar").HasKey(e => e.IdLugar);
                modelBuilder.Entity<LUGAR_TURISTICO>().Property(u => u.Nombre).HasColumnName("Nombre").IsRequired();
                modelBuilder.Entity<LUGAR_TURISTICO>().Property(u => u.Ciudad).HasColumnName("ciudad").IsRequired();
                modelBuilder.Entity<LUGAR_TURISTICO>().Property(u => u.Descripcion).HasColumnName("descripcion").IsRequired();
                modelBuilder.Entity<LUGAR_TURISTICO>().Property(u => u.Imagen).HasColumnName("imagen").IsRequired();
                modelBuilder.Entity<LUGAR_TURISTICO>().Property(u => u.ubicacion).HasColumnName("ubicacion").IsRequired();
            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }

        }
    }


