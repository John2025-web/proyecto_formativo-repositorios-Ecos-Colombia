using ApiPrueba.DBcontext;
using ApiPrueba.Models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Repositorios    
{
    public class Tipo_ContenidoRepository : ITipo_ContenidoRepo
    {
        private readonly Tipo_ContenidoService context;

        public Tipo_ContenidoRepository(Tipo_ContenidoService context)
        {
            this.context = context;
        }

        public async Task<List<Tipo_Contenido>> GetTipo_Contenido()
        {
            var data = await context.tipo_Contenido.ToListAsync();
            return data;
        }

        public async Task<Tipo_Contenido?> GetTipo_ContenidoById(int id)
        {
            var data = await context.tipo_Contenido.FirstOrDefaultAsync(x => x.idTipo == id);
            return data;
        }

        public async Task<bool> PostTipo_Contenido(Tipo_Contenido tipo_Contenido)
        {
            await context.tipo_Contenido.AddAsync(tipo_Contenido);
            await context.SaveAsync();
            return true;
        }

        public async Task<bool> PutTipo_Contenido(Tipo_Contenido tipo_Contenido)
        {
            context.tipo_Contenido.Update(tipo_Contenido);
            await context.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteTipo_Contenido(Tipo_Contenido tipo_Contenido)
        {
            context.tipo_Contenido.Remove(tipo_Contenido);
            await context.SaveAsync();
            return true;

        }
    }
}
