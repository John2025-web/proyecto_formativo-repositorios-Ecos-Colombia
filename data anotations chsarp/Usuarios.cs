using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace tablas_ecoscolombia
{
    public class Usuario
    {
        static void Main(string[] args)
        { 
            public int idUsuario { get; set; }
            public string Nombre { get; set; }
            public string Correo { get; set; }
            public string PaisOrigen { get; set; }
            public string IdiomaPreferido { get; set; }
            public string TipoUsuario { get; set; } 

            public Idioma Idioma {  get; set; }
        }
    } 
}
public void EntityConfiguration(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Usuario>().ToTable("Usuario");

    modelBuilder.Entity<Usuario>()
        .HasKey(u => u.idUsuario);

    modelBuilder.Entity<Usuario>()
        .Property(u => u.idUsuario)
        .HasColumnName("idUsuario")
        .ValueGeneratedOnAdd();

    modelBuilder.Entity<Usuario>()
        .Property(u => u.Nombre)
        .HasColumnName("Nombre")
        .HasMaxLength(100)
        .IsRequired();

    modelBuilder.Entity<Usuario>()
        .Property(u => u.Correo)
        .HasColumnName("Correo")
        .HasMaxLength(150)
        .IsRequired();

    modelBuilder.Entity<Usuario>()
        .Property(u => u.PaisOrigen)
        .HasColumnName("PaisOrigen")
        .HasMaxLength(100);

    modelBuilder.Entity<Usuario>()
        .Property(u => u.IdiomaPreferido)
        .HasColumnName("IdiomaPreferido")
        .HasMaxLength(50);

    modelBuilder.Entity<Usuario>()
        .Property(u => u.TipoUsuario)
        .HasColumnName("TipoUsuario")
        .HasMaxLength(50);

    modelBuilder.Entity<Usuario>()
        .HasOne(u => u.Idioma)
        .WithMany()
        .HasForeignKey(u => u.idIdioma)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Usuario>()
        .HasMany(u => u.Resenas)
        .WithOne(r => r.Usuario)
        .HasForeignKey(r => r.idUsuario);

    modelBuilder.Entity<Usuario>()
        .HasMany(u => u.Notificaciones)
        .WithOne(n => n.Usuario)
        .HasForeignKey(n => n.idUsuario);
}


 
