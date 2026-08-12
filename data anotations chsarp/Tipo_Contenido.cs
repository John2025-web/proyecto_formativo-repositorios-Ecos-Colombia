using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace tablas_ecoscolombia
{
    public class Tipo_Contenido
    {
        [Key]
        public int idContenido { get; set; }

        public string Nombre { get; set; }

        public string Descipcion { get; set; } 

    }

    public static class TipoContenidoConfiguration
    {
        public static void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tipo_Contenido>().ToTable("Tipo_Contenido");

            modelBuilder.Entity<Tipo_Contenido>()
                .HasKey(t => t.idContenido);

            modelBuilder.Entity<Tipo_Contenido>()
                .Property(t => t.idContenido)
                .HasColumnName("idContenido")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Tipo_Contenido>()
                .Property(t => t.Nombre)
                .HasColumnName("Nombre")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Tipo_Contenido>()
                .Property(t => t.Descipcion)
                .HasColumnName("Descripcion")
                .HasMaxLength(250);
        }
    }
}
