using Microsoft.EntityFrameworkCore;
using ApiPrueba.Models;

namespace ApiPrueba.DBcontext
{
    public class NotificacionService : DbContext
    {
        public NotificacionService(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Notificacion> Notificacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }
        private void EntityConfiguration(ModelBuilder modelBuilder)
        {
         
            modelBuilder.Entity<Notificacion>().ToTable("Notificacion").HasKey(n => n.idNotificacion);

            modelBuilder.Entity<Notificacion>().Property(n => n.idUsuario).HasColumnName("idUsuario").IsRequired();

            modelBuilder.Entity<Notificacion>().Property(n => n.Mensaje).HasColumnName("Mensaje").HasMaxLength(300).IsRequired();

            modelBuilder.Entity<Notificacion>().Property(n => n.FechaEnvio).HasColumnName("FechaEnvio").IsRequired();

            modelBuilder.Entity<Notificacion>().Property(n => n.Tipo).HasColumnName("Tipo").HasMaxLength(50);

            modelBuilder.Entity<Notificacion>().HasOne(n => n.Usuario).WithMany().HasForeignKey(n => n.idUsuario).OnDelete(DeleteBehavior.Cascade);
            
        }
    }

}

