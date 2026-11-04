using ApiPrueba.DBcontex;
using ApiPrueba.models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.repositorios
{
    public class culturatradiRepository : IculturaTradicionRepo
    {
        private readonly cultura_TradicionService context;

        public culturatradiRepository(cultura_TradicionService context)
        {
            this.context = context;
        }
        public async Task<List<culturaTradicion>> GetculturaTradicion()
        {
            var data = await context.culturaTradicion.ToListAsync();
            return data;
        }
        public async Task<culturaTradicion?> GetculturaTradicionById(int id)
        {
            var data = await context.culturaTradicion.FirstOrDefaultAsync(X => X.idCultura == id);
            return data;
        }
        public async Task<bool> PostculturaTradicion(culturaTradicion culturaTradicion)
        {
            await context.culturaTradicion.AddAsync(culturaTradicion);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> PutculturaTradicion(culturaTradicion culturaTradicion)
        {
            context.culturaTradicion.Update(culturaTradicion);
            await context.SaveAsync();
            return true;
        }
        public async Task<bool> DeleteculturaTradicion(culturaTradicion culturaTradicion)
        {
            context.culturaTradicion.Remove(culturaTradicion);
            await context.SaveAsync();
            return true;
        }
    }
}
