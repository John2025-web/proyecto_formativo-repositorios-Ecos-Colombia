using Microsoft.EntityFrameworkCore;
using ApiPrueba.DBcontex;
using ApiPrueba.models;
using ApiPrueba.Repositorios.Interfaces;

namespace ApiPrueba.repositorios
   
{
    public class LugarTuRepository : ILugarTuristicoRepo
    {

        private readonly lugar_turistico_Service context;

        public LugarTuRepository(lugar_turistico_Service context)
        {
            this.context = context;
        }
        public async Task<List<LUGAR_TURISTICO>> GetLugarTuristico()
        {
            var data = await context.lugar_Turistico.ToListAsync();
            return data;
        }
        public async Task<LUGAR_TURISTICO?> GetLugarTuristicoById(int id)
        {
            var data = await context.lugar_Turistico.FirstOrDefaultAsync(X => X. IdLugar == id);
            return data;
        }
        public async Task<bool> PostLugarTuristico(LUGAR_TURISTICO LugarTuristico)
        {
            await context.lugar_Turistico.AddAsync(LugarTuristico);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> PutLugarTuristico(LUGAR_TURISTICO LugarTuristico)
        {
            context.lugar_Turistico.Update(LugarTuristico);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteLugarTuristico(LUGAR_TURISTICO LugarTuristico)
        {
            context.lugar_Turistico.Remove(LugarTuristico);
            await context.SaveChangesAsync();
            return true;
        }
    }
}

