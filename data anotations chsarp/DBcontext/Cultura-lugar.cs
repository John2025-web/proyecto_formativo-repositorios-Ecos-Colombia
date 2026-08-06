using Microsoft.EntityFrameworkCore; 
using pruebaApi.models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace pruebaApi.dbcontext

{
    public class DatabaseService : DbContext
    {
        public DatabaseService(DbContextOptions options): base(options) 
        { 
        }
        public DbSet<culturaLugar> CulturaLugar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }

    private void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<culturaLugar>().ToTable("culturaLugar");
            modelBuilder.Entity<culturaLugar>().HasKey(ct => new { ct.idCultura, ct.idLugar });
            modelBuilder.Entity<culturaLugar>().Property(u=> u.idCultura).HasColumnName("id_culturaLugar").ValueGeneratedOnAdd();
            modelBuilder.Entity<culturaLugar>().Property(u => u.idLugar).HasColumnName("id_lugar").ValueGeneratedOnAdd();

        }
        public async Task<bool> SaveAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }

    }
}





