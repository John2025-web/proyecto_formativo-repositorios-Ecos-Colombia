using ApiPrueba.DBcontext;
using ApiPrueba.Models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Repositorios
{
    public class RecenaRepository : IRecenaRepo
    {
        private readonly RecenaService context;

        public RecenaRepository(RecenaService context)
        {
            this.context = context;
        }

        public async Task<List<Recena>> GetRecena()
        {
            var data = await context.recena.ToListAsync();
            return data;
        }

        public async Task<Recena?> GetRecenaById(int id)
        {
            var data = await context.recena.FirstOrDefaultAsync(x => x.idRecena == id);
            return data;
        }

        public async Task<bool> PostResena(Recena recena)
        {
            await context.recena.AddAsync(recena);
            await context.SaveAsync();
            return true;
        }

        public async Task<bool> PutResena(Recena resena)
        {
            context.recena.Update(resena);
            await context.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteResena(Recena resena)
        {
            context.recena.Remove(resena);
            await context.SaveAsync();
            return true;
        }

        public Task<bool> PostRecena(Recena recena)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PutRecena(Recena recena)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRecena(Recena recena)
        {
            throw new NotImplementedException();
        }
    }
}
