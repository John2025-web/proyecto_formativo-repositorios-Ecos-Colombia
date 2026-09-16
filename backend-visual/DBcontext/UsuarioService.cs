using Microsoft.EntityFrameworkCore;
using ApiPrueba.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ApiPrueba.DBcontext;

    public class UsuarioService : DbContext
    {
        public UsuarioService(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Usuario> usuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }
        private void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("Usuario").HasKey(u => u.idUsuario);

            modelBuilder.Entity<Usuario>().Property(u => u.Nombre).HasColumnName("Nombre").HasMaxLength(100).IsRequired();

            modelBuilder.Entity<Usuario>().Property(u => u.Correo).HasColumnName("Correo").HasMaxLength(150).IsRequired();

            modelBuilder.Entity<Usuario>().Property(u => u.PaisOrigen).HasColumnName("PaisOrigen").HasMaxLength(100);

            modelBuilder.Entity<Usuario>().Property(u => u.IdiomaPreferido).HasColumnName("IdiomaPreferido").HasMaxLength(50);

            modelBuilder.Entity<Usuario>().Property(u => u.TipoUsuario).HasColumnName("TipoUsuario").HasMaxLength(50);

            modelBuilder.Entity<Usuario>().Property(u => u.PasswordHash).HasColumnName("Contraseña").HasMaxLength(100).IsRequired();

    }
    public async Task<bool> SaveAsync()
    {
        return await base.SaveChangesAsync() > 0;
    }
}
