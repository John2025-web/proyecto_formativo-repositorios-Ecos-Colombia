using Microsoft.EntityFrameworkCore;
using ApiPrueba.Models;

namespace ApiPrueba.DBcontext
{
    public class RecenaService : DbContext
    {
        public RecenaService(DbContextOptions<RecenaService> options) : base(options)
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

            modelBuilder.Entity<Recena>().Property(r => r.idRecena).HasColumnName("id_resena").ValueGeneratedOnAdd();

            modelBuilder.Entity<Recena>().Property(r => r.Comentario).HasColumnName("Comentario").HasMaxLength(500);

            modelBuilder.Entity<Recena>().Property(r => r.Calificacion).HasColumnName("Calificacion").IsRequired();

            modelBuilder.Entity<Recena>().Property(r => r.Fecha).HasColumnName("Fecha");
            modelBuilder.Entity<Recena>().Property(r => r.Visible).HasColumnName("Visible").IsRequired();

            modelBuilder.Entity<Recena>().Property(r => r.idUsuario).HasColumnName("id_usuario");
            modelBuilder.Entity<Recena>().Property(r => r.idLugar).HasColumnName("id_lugar");
        }
        public async Task<bool> SaveAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}


