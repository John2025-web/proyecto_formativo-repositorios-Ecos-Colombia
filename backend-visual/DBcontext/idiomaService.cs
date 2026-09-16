using Microsoft.EntityFrameworkCore;
using ApiPrueba.models;

namespace ApiPrueba.DBcontex
{
    
        public class IdiomaService : DbContext
        {
            public IdiomaService(DbContextOptions options) : base(options)
            {
            }
            public DbSet<IDIOMA> Idioma { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                EntityConfiguration(modelBuilder);
            }
            private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<IDIOMA>().ToTable("IDIOMA").HasKey(e => e.CodigoIso);
                modelBuilder.Entity<IDIOMA>().Property(u => u.IdiomaNombre).HasColumnName("IdiomaNombre").IsRequired();
            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }

        }
    }