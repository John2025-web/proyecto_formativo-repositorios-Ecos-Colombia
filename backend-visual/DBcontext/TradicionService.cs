using Microsoft.EntityFrameworkCore;
using ApiPrueba.Models;

namespace ApiPrueba.DBcontext
{
    public class TradicionService : DbContext
    {
        public TradicionService(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Tradicion> tradicion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfiguration(modelBuilder);
        }
        private void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tradicion>().ToTable("Tradicion").HasKey(t => t.idTradicion);

            modelBuilder.Entity<Tradicion>().Property(t => t.Nombre).HasColumnName("Nombre").HasMaxLength(150).IsRequired();

            modelBuilder.Entity<Tradicion>().Property(t => t.idRegion).HasColumnName("idRegion").IsRequired();

            modelBuilder.Entity<Tradicion>().Property(t => t.FechaCelebracion).HasColumnName("FechaCelebracion");

            modelBuilder.Entity<Tradicion>().HasOne(t => t.Region).WithMany(r => r.Tradiciones).HasForeignKey(t => t.idRegion).OnDelete(DeleteBehavior.Restrict);
        }
        public async Task<bool> SaveAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }

    }
}
