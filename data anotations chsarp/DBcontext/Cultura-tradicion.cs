using Microsoft.EntityFrameworkCore;
using pruebaApi.models;

namespace pruebaApi.DBcontext
{
    public class Cultura_tradicion
    {
        public class DatabaseService : DbContext
        {
            public DatabaseService(DbContextOptions options) : base(options)
            {
            }
            public DbSet<culturaTradicion> CulturaTradicion { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                EntityConfiguration(modelBuilder);
            }

            private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<culturaTradicion>().ToTable("culturaTradicion");
                modelBuilder.Entity<culturaTradicion>().HasKey(ct => new { ct.idCultura, ct.idTradicion });
                modelBuilder.Entity<culturaTradicion>().Property(u => u.idCultura).HasColumnName("id_culturaTradicion").ValueGeneratedOnAdd();
                modelBuilder.Entity<culturaTradicion>().Property(u => u.idTradicion).HasColumnName("id_Tradicion").ValueGeneratedOnAdd();

            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }
        }
    }
}
