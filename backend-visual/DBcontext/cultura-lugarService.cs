using Microsoft.EntityFrameworkCore;
using ApiPrueba.models;

namespace ApiPrueba.DBcontex
{ 

        public class cultura_LugarService : DbContext
        {
            public cultura_LugarService(DbContextOptions options) : base(options)
            {
            }
            public DbSet<culturaLugar> cultura_Lugar { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                EntityConfiguration(modelBuilder);
            }
            private void EntityConfiguration(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<culturaLugar>()
                .ToTable("CulturaLugar")
                .HasKey(u => new { u.idCultura, u.idLugar });
            }
            public async Task<bool> SaveAsync()
            {
                return await base.SaveChangesAsync() > 0;
            }

        }
}

