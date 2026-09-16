using ApiPrueba.models;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ADMINISTRADORES> ADMINISTRADORES { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ADMINISTRADORES>().ToTable("ADMINISTRADORES");
            modelBuilder.Entity<ADMINISTRADORES>().HasKey(x => x.IdAdmin);
            modelBuilder.Entity<ADMINISTRADORES>().Property(x => x.IdAdmin).HasColumnName("id_admin").ValueGeneratedOnAdd();
            modelBuilder.Entity<ADMINISTRADORES>().Property(x => x.Nombre).HasColumnName("nombre");
            modelBuilder.Entity<ADMINISTRADORES>().Property(x => x.Correo).HasColumnName("correo");

            ConfigureContenido(modelBuilder);
        }

        private void ConfigureContenido(ModelBuilder modelBuilder)
        {}
        public async Task<bool> SaveChangesBoolAsync()
        {
            var cambios = await SaveChangesAsync();
            return cambios > 0;
        }
    }
}