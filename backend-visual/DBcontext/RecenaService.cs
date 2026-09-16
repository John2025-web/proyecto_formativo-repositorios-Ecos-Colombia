using Microsoft.EntityFrameworkCore;
using ApiPrueba.Models;

namespace ApiPrueba.DBcontext
{
    public class RecenaService : DbContext
    {
        public RecenaService(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Recena> recena { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }
        private void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recena>().ToTable("Resena").HasKey(r => r.idRecena);

            modelBuilder.Entity<Recena>().Property(r => r.idRecena).HasColumnName("idRecena").ValueGeneratedOnAdd();

            modelBuilder.Entity<Recena>().Property(r => r.Comentario).HasColumnName("Comentario").HasMaxLength(500);

            modelBuilder.Entity<Recena>().Property(r => r.Calificacion).HasColumnName("Calificacion").IsRequired();

            modelBuilder.Entity<Recena>().Property(r => r.Fecha).HasColumnName("Fecha");

            modelBuilder.Entity<Recena>().HasOne(r => r.Usuario).WithMany().HasForeignKey(r => r.idUsuario).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Recena>().Property(r => r.idLugar).HasColumnName("idLugar");
        }
        public async Task<bool> SaveAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}


