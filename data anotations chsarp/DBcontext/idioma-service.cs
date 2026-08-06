using Microsoft.EntityFrameworkCore;
using pruebaApi.models;

namespace pruebaApi.DBcontext
{
    public class idioma_service
    {
        public class DatabaseService : DbContext
        {
            public DatabaseService(DbContextOptions options) : base(options)
            {
            }
            public DbSet<IDIOMA> IDIOMA { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                EntityConfiguration(modelBuilder);
            }

            private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<IDIOMA>().ToTable("IDIOMA   ");
                modelBuilder.Entity<IDIOMA>().HasKey(u => u.CodigoIso);
                modelBuilder.Entity<IDIOMA>().Property(u => u.IdiomaNombre).HasColumnName("id_culturaLugar").ValueGeneratedOnAdd();
                

            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }

        }
    }
}

