using ApiPrueba.Data;
using ApiPrueba.models;
using ApiPrueba.Repositories.@interface;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Repositories
{
    public class CULTURARepository : ICULTURARepository
    {
        private readonly DatabaseServiceCULTURA context;

        public CULTURARepository(DatabaseServiceCULTURA context)
        {
            this.context = context;
        }

        public async Task<List<CULTURA>> GetCulturas()
        {
            var data = await context.CULTURA.ToListAsync();
            return data;
        }

        public async Task<CULTURA> GetCulturaById(int id)
        {
            var data = await context.CULTURA.FirstOrDefaultAsync(x => x.IdCultura == id);
            return data;
        }

        public async Task<bool> PostCultura(CULTURA cultura)
        {
            await context.CULTURA.AddAsync(cultura);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PutCultura(CULTURA cultura)
        {
            context.CULTURA.Update(cultura);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCultura(CULTURA cultura)
        {
            context.CULTURA.Remove(cultura);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
