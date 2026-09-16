using ApiPrueba.Data;
using ApiPrueba.models;
using ApiPrueba.Repositories.@interface;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Repositories
{
    public class CONTENIDO_LUGARRepository : ICONTENIDO_LUGARRepository
    {
        private readonly DatabaseServiceCONTENIDO_LUGAR context;

        public CONTENIDO_LUGARRepository(DatabaseServiceCONTENIDO_LUGAR context)
        {
            this.context = context;
        }

        public async Task<List<CONTENIDO_LUGAR>> GetContenidoLugar()
        {
            var data = await context.CONTENIDO_LUGAR.ToListAsync();
            return data;
        }

        public async Task<CONTENIDO_LUGAR> GetContenidoLugarById(int idContenido, int idLugar)
        {
            var data = await context.CONTENIDO_LUGAR.FirstOrDefaultAsync(x => x.IdContenido == idContenido && x.IdLugar == idLugar);
            return data;
        }

        public async Task<bool> PostContenidoLugar(CONTENIDO_LUGAR contenidoLugar)
        {
            await context.CONTENIDO_LUGAR.AddAsync(contenidoLugar);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PutContenidoLugar(CONTENIDO_LUGAR contenidoLugar)
        {
            context.CONTENIDO_LUGAR.Update(contenidoLugar);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteContenidoLugar(CONTENIDO_LUGAR contenidoLugar)
        {
            context.CONTENIDO_LUGAR.Remove(contenidoLugar);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
