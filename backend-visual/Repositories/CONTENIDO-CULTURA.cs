using ApiPrueba.Data;
using ApiPrueba.models;
using ApiPrueba.Repositories.@interface;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Repositories
{
    public class CONTENIDO_CULTURARepository : ICONTENIDO_CULTURARepository
    {
        private readonly DatabaseServiceCONTENIDO_CULTURA context;

        public CONTENIDO_CULTURARepository(DatabaseServiceCONTENIDO_CULTURA context)
        {
            this.context = context;
        }

        public async Task<List<CONTENIDO_CULTURA>> GetContenidoCultura()
        {
            var data = await context.CONTENIDO_CULTURA.ToListAsync();
            return data;
        }

        public async Task<CONTENIDO_CULTURA> GetContenidoCulturaById(int idContenido, int idCultura)
        {
            var data = await context.CONTENIDO_CULTURA.FirstOrDefaultAsync(x => x.IdContenido == idContenido && x.IdCultura == idCultura);
            return data;
        }

        public async Task<bool> PostContenidoCultura(CONTENIDO_CULTURA contenidoCultura)
        {
            await context.CONTENIDO_CULTURA.AddAsync(contenidoCultura);
            await context.SaveChangesAsync();
            return true;
        }   

        public async Task<bool> PutContenidoCultura(CONTENIDO_CULTURA contenidoCultura)
        {
            context.CONTENIDO_CULTURA.Update(contenidoCultura);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteContenidoCultura(CONTENIDO_CULTURA contenidoCultura)
        {
            context.CONTENIDO_CULTURA.Remove(contenidoCultura);
            await context.SaveChangesAsync();
            return true;
        }
    }
}