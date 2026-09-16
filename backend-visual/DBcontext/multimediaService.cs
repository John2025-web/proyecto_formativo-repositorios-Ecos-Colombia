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
                modelBuilder.Entity<MULTIMEDIA>().ToTable("Idmultimedia").HasKey(u => u.IdMultimedia);
                modelBuilder.Entity<MULTIMEDIA>().ToTable("Idcontenido").HasKey(u => u.IdContenido);
                modelBuilder.Entity<MULTIMEDIA>().Property(u => u.tipoContenido).HasColumnName("Tipocontenido").IsRequired();
                modelBuilder.Entity<MULTIMEDIA>().Property(u => u.url).HasColumnName("url").IsRequired();
            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }

        }
    }

