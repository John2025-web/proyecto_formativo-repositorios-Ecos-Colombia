using ApiPrueba.Data;
using ApiPrueba.models;
using ApiPrueba.Repositories.@interface;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Repositories
{
    public class ADMINISTRADORESRepository : IADMINISTRADORESRepository
    {
        private readonly AppDbContext context;

        public ADMINISTRADORESRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<ADMINISTRADORES>> GetAdministradores()
        {
            return await context.ADMINISTRADORES.ToListAsync();
        }

        public async Task<ADMINISTRADORES?> GetAdministradorById(int id)
        {
            return await context.ADMINISTRADORES
                .FirstOrDefaultAsync(x => x.IdAdmin == id);
        }

        public async Task<bool> PostAdministrador(ADMINISTRADORES admin)
        {
            await context.ADMINISTRADORES.AddAsync(admin);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PutAdministrador(ADMINISTRADORES admin)
        {
            context.ADMINISTRADORES.Update(admin);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAdministrador(ADMINISTRADORES admin)
        {
            context.ADMINISTRADORES.Remove(admin);
            await context.SaveChangesAsync();
            return true;
        }
    }
}