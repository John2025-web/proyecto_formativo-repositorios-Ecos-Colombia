using Microsoft.EntityFrameworkCore;
using ApiPrueba.models;

namespace ApiPrueba.DBcontex
{
        public class MultimediaService : DbContext
        {
            public MultimediaService(DbContextOptions options) : base(options)
            {
            }
            public DbSet<MULTIMEDIA> multimedia { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                EntityConfiguration(modelBuilder);
            }
            private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<MULTIMEDIA>().ToTable("MULTIMEDIA").HasKey(u => u.IdMultimedia);
                modelBuilder.Entity<MULTIMEDIA>().Property(u => u.IdMultimedia).ValueGeneratedOnAdd();
                modelBuilder.Entity<MULTIMEDIA>().Property(u => u.IdContenido).HasColumnName("id_contenido");
                modelBuilder.Entity<MULTIMEDIA>().Property(u => u.tipoContenido).HasColumnName("tipo").IsRequired();
                modelBuilder.Entity<MULTIMEDIA>().Property(u => u.url).HasColumnName("url").IsRequired();
            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }

        }
    }

