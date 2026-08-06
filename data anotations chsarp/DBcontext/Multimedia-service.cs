using Microsoft.EntityFrameworkCore;
using pruebaApi.models;

namespace pruebaApi.DBcontext
{
    public class MultimediaService
    {
        public class DatabaseService : DbContext
        {
            public DatabaseService(DbContextOptions options) : base(options)
            {
            }

            public DbSet<Multimedia> Multimedia { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                EntityConfiguration(modelBuilder);
            }

            private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Multimedia>().ToTable("Multimedia");
                modelBuilder.Entity<Multimedia>().HasKey(u => u.IdMultimedia);
                modelBuilder.Entity<Multimedia>().HasKey(u => u.IdContenido);
                modelBuilder.Entity<Multimedia>().Property(u => u.tipoContenido).HasColumnName("tipo contenido").ValueGeneratedOnAdd();
                modelBuilder.Entity<Multimedia>().Property(u => u.url).HasColumnName("url").ValueGeneratedOnAdd();
            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }

        }
    }
}