using ApiPrueba.DBcontext;
using ApiPrueba.Models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace ApiPrueba.Repositorios    
{
    public class TradicionRepository : ITradicionRepo
    {
        private readonly TradicionService context;

        public TradicionRepository(TradicionService context)
        {
            this.context = context;
        }

        public async Task<List<Tradicion>> GetTradicion()
        {
            var data = await context.tradicion.ToListAsync();
            return data;
        }

        public async Task<Tradicion?> GetTradicionById(int id)
        {
            var data = await context.tradicion.FirstOrDefaultAsync(x => x.idTradicion == id);
            return data;
        }

        public async Task<bool> PostTradicion(Tradicion tradicion)
        {
            await context.tradicion.AddAsync(tradicion);
            await context.SaveAsync();
            return true;
        }

        public async Task<bool> PutTradicion(Tradicion tradicion)
        {
            context.tradicion.Update(tradicion);
            await context.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteTradicion(Tradicion tradicion)
        {
            context.tradicion.Remove(tradicion);
            await context.SaveAsync();
            return true;
        }
    }
}
