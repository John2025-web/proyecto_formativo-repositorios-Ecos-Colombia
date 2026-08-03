using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tablas_ecoscolombia
{
    public class Tradicion
    {
        public int idTradicion { get; set; }
        public string Nombre { get; set; }
        public int idRegion { get; set; }
        public DateTime? FechaCelebracion { get; set; }
        
        public Region Region { get; set; }
    }
}
public void EntityConfiguration(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Tradicion>().ToTable("Tradicion");

    modelBuilder.Entity<Tradicion>()
        .HasKey(t => t.idTradicion);

    modelBuilder.Entity<Tradicion>()
        .Property(t => t.idTradicion)
        .HasColumnName("idTradicion")
        .ValueGeneratedOnAdd();

    modelBuilder.Entity<Tradicion>()
        .Property(t => t.Nombre)
        .HasColumnName("Nombre")
        .HasMaxLength(150)
        .IsRequired();

    modelBuilder.Entity<Tradicion>()
        .Property(t => t.idRegion)
        .HasColumnName("idRegion")
        .IsRequired();

    modelBuilder.Entity<Tradicion>()
        .Property(t => t.FechaCelebracion)
        .HasColumnName("FechaCelebracion");

    modelBuilder.Entity<Tradicion>()
        .HasOne(t => t.Region)
        .WithMany()
        .HasForeignKey(t => t.idRegion)
        .OnDelete(DeleteBehavior.Restrict);
}
