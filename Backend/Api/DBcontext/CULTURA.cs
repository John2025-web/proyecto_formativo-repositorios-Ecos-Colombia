using ApiPrueba.models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ApiPrueba.Data
{
    public class DatabaseServiceCULTURA : DbContext
    {
        public DatabaseServiceCULTURA(DbContextOptions<DatabaseServiceCULTURA> options) : 
            base(options)
        {
        }

        public DbSet<CULTURA> CULTURA { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }

        private void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CULTURA>().ToTable("CULTURA");

            modelBuilder.Entity<CULTURA>().HasKey(u => u.IdCultura);

            modelBuilder.Entity<CULTURA>()
                .Property(u => u.IdCultura)
                .HasColumnName("id_cultura")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<CULTURA>()
                .Property(u => u.Nombre)
                .HasColumnName("nombre");

            modelBuilder.Entity<CULTURA>()
                .Property(u => u.Descripcion)
                .HasColumnName("descripcion");

            modelBuilder.Entity<CULTURA>()
                .Property(u => u.IdCategoria)
                .HasColumnName("idCategoria");
        }

        public async Task<bool> SaveAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
