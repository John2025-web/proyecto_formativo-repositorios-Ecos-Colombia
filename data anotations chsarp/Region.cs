using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace tablas_ecoscolombia
{
    public class Region
    {
        [Key]
        public int idRegion { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        public List<Tradicion> Tradiciones { get; set; }
    }

    public static class RegionConfiguration
    {
        public static void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Region>().ToTable("Region");

            modelBuilder.Entity<Region>()
                .HasKey(r => r.idRegion);

            modelBuilder.Entity<Region>()
                .Property(r => r.idRegion)
                .HasColumnName("idRegion")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Region>()
                .Property(r => r.Nombre)
                .HasColumnName("Nombre")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Region>()
                .HasMany(r => r.Tradiciones)
                .WithOne(t => t.Region)
                .HasForeignKey(t => t.idRegion)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
