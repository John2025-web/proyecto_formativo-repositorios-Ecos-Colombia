using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tablas_ecoscolombia
{
    public class Notificacion
    {

        [Key]
        public int idNotificacion { get; set; }
        public int idUsuario { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Tipo { get; set; }


        [ForeignKey("idUsuario")]
        public Usuario Usuario {get; set;}

        
    }
}
public void EntityConfiguration(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Notificacion>().ToTable("Notificacion");

    modelBuilder.Entity<Notificacion>()
        .HasKey(n => n.idNotificacion);

    modelBuilder.Entity<Notificacion>()
        .Property(n => n.idNotificacion)
        .HasColumnName("idNotificacion")
        .ValueGeneratedOnAdd();

    modelBuilder.Entity<Notificacion>()
        .Property(n => n.idUsuario)
        .HasColumnName("idUsuario")
        .IsRequired();

    modelBuilder.Entity<Notificacion>()
        .Property(n => n.Mensaje)
        .HasColumnName("Mensaje")
        .HasMaxLength(300)
        .IsRequired();

    modelBuilder.Entity<Notificacion>()
        .Property(n => n.FechaEnvio)
        .HasColumnName("FechaEnvio")
        .IsRequired();

    modelBuilder.Entity<Notificacion>()
        .Property(n => n.Tipo)
        .HasColumnName("Tipo")
        .HasMaxLength(50);

    modelBuilder.Entity<Notificacion>()
        .HasOne(n => n.Usuario)
        .WithMany()
        .HasForeignKey(n => n.idUsuario)
        .OnDelete(DeleteBehavior.Cascade);
}
