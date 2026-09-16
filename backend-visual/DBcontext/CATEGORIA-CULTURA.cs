using ApiPrueba.models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ApiPrueba.Data
{
    public class DatabaseServiceCATEGORIA_CULTURA : DbContext
    {
        public DatabaseServiceCATEGORIA_CULTURA(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CATEGORIA_CULTURA> CATEGORIA_CULTURA { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfigurationCategoria(modelBuilder);
            EntityConfigurationCultura(modelBuilder); 
        }

        private void EntityConfigurationCategoria(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CATEGORIA_CULTURA>().ToTable("CATEGORIA_CULTURA");
            modelBuilder.Entity<CATEGORIA_CULTURA>().HasKey(u => u.IdCategoria);
            modelBuilder.Entity<CATEGORIA_CULTURA>().Property(u => u.IdCategoria).HasColumnName("id_categoria").ValueGeneratedOnAdd();
            modelBuilder.Entity<CATEGORIA_CULTURA>().Property(u => u.Nombre).HasColumnName("nombre");
        }

        private void EntityConfigurationCultura(ModelBuilder modelBuilder)
        {}

        public async Task<bool> SaveAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}