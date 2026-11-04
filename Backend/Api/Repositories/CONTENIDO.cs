using ApiPrueba.Data;
using ApiPrueba.models;
using ApiPrueba.Repositories.@interface;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Repositories
{
    public class CONTENIDORepository : ICONTENIDORepository
    {
        private readonly DatabaseServiceCONTENIDO context;

        public CONTENIDORepository(DatabaseServiceCONTENIDO context)
        {
            this.context = context;
        }

        public async Task<List<CONTENIDO>> GetContenidos()
        {
            var data = await context.CONTENIDO.ToListAsync();
            return data;
        }

        public async Task<CONTENIDO?> GetContenidoById(int id)
        {
            var data = await context.CONTENIDO.FirstOrDefaultAsync(x => x.IdContenido == id);
            return data;
        }

        public async Task<bool> PostContenido(CONTENIDO contenido)
        {
            await context.CONTENIDO.AddAsync(contenido);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PutContenido(CONTENIDO contenido)
        {
            context.CONTENIDO.Update(contenido);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteContenido(CONTENIDO contenido)
        {
            context.CONTENIDO.Remove(contenido);
            await context.SaveChangesAsync();
            return true;
        }
    }
}