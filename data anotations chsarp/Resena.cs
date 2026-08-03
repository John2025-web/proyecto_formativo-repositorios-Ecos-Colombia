using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace tablas_ecoscolombia
{
    public class Resena
    {
        [Key]
        public int idResena {  get; set; }
        
        public int idUsuario { get; set; }
        public int idLugar { get; set; }
        
        public string Comentario { get; set; }

        [System.ComponentModel.DataAnnotations.Range(1,5)]
        public int Calificacion { get; set; }
        public DateTime? Fecha { get; set; }

        [ForeignKey("idUsuario")]
        public Usuario Usuario { get; set; }

        [ForeignKey("idLugar")]
        public LugarTuristico Lugar { get; set; }
    }
}
    public void EntityConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Resena>().ToTable("Resena");

        modelBuilder.Entity<Resena>().HasKey(r => r.idResena);

        modelBuilder.Entity<Resena>()
            .Property(r => r.idResena)
            .HasColumnName("idResena")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Resena>()
            .Property(r => r.Comentario)
            .HasColumnName("Comentario")
            .HasMaxLength(500);

        modelBuilder.Entity<Resena>()
            .Property(r => r.Calificacion)
            .HasColumnName("Calificacion")
            .IsRequired();

        modelBuilder.Entity<Resena>()
            .Property(r => r.Fecha)
            .HasColumnName("Fecha");

        modelBuilder.Entity<Resena>()
            .HasOne(r => r.Usuario)
            .WithMany()
            .HasForeignKey(r => r.idUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Resena>()
            .HasOne(r => r.Lugar)
            .WithMany()
            .HasForeignKey(r => r.idLugar)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public async Task<bool> SaveAsync()
    {
        return await SaveChangesAsync() > 0;
    }
}
