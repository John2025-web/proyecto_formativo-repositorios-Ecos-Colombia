using ApiPrueba.DBcontex;
using ApiPrueba.models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.repositorios
{
    public class IdiomaRepository : IidiomaRepo
    {
        private readonly IdiomaService context;

        public IdiomaRepository(IdiomaService context)
        {
            this.context = context;
        }
        public async Task<List<IDIOMA>> GetIDIOMA()
        {
            var data = await context.Idioma.ToListAsync();
            return data;
        }
        public async Task<IDIOMA?> GetIDIOMAById(int id)
        {
            var data = await context.Idioma.FirstOrDefaultAsync(X => X.CodigoIso == id);
            return data;
        }
        public async Task<bool> PostIDIOMA(IDIOMA IDIOMA)
        {
            await context.Idioma.AddAsync(IDIOMA);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> PutIDIOMA(IDIOMA IDIOMA)
        {
            context.Idioma.Update(IDIOMA);
            await context.SaveAsync();
            return true;
        }
        public async Task<bool> DeleteIDIOMA(IDIOMA IDIOMA)
        {
            context.Idioma.Remove(IDIOMA);
            await context.SaveAsync();
            return true;
        }
    }
}
