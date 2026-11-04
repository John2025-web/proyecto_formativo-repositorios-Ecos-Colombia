using ApiPrueba.Data;
using ApiPrueba.models;
using ApiPrueba.Repositories.@interface;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Repositories
{   public class CATEGORIA_CULTURARepository : ICATEGORIA_CULTURARepository
    {
        private readonly DatabaseServiceCATEGORIA_CULTURA context;
        public CATEGORIA_CULTURARepository(DatabaseServiceCATEGORIA_CULTURA context)
        {
            this.context = context;
        }
        public async Task<List<CATEGORIA_CULTURA>> GetCategorias()
        {
            var data = await context.CATEGORIA_CULTURA.ToListAsync();
            return data;
        }
        public async Task<CATEGORIA_CULTURA> GetCategoriaById(int id)
        {
            var data = await context.CATEGORIA_CULTURA
                .FirstOrDefaultAsync(x => x.IdCategoria == id);

            return data;
        }
        public async Task<bool> PostCategoria(CATEGORIA_CULTURA categoria)
        {
            await context.CATEGORIA_CULTURA.AddAsync(categoria);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> PutCategoria(CATEGORIA_CULTURA categoria)
        {
            context.CATEGORIA_CULTURA.Update(categoria);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteCategoria(CATEGORIA_CULTURA categoria)
        {
            context.CATEGORIA_CULTURA.Remove(categoria);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
