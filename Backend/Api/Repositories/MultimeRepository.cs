using ApiPrueba.DBcontex;
using ApiPrueba.models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.repositorios
{
    public class MultimeRepository : IMultimediaRepo
    {
        
            private readonly MultimediaService context;

            public MultimeRepository(MultimediaService context)
            {
                this.context = context;
            }
            public async Task<List<MULTIMEDIA>> GetMultimedia()
            {
                var data = await context.multimedia.ToListAsync();
                return data;
            }
            public async Task<MULTIMEDIA?> GetMultimediaById(int id)
            {
                var data = await context.multimedia.FirstOrDefaultAsync(X => X.IdMultimedia == id);
                return data;
            }
            public async Task<bool> PostMultimedia(MULTIMEDIA Multimedia)
            {
                await context.multimedia.AddAsync(Multimedia);
                await context.SaveChangesAsync();
                return true;
            }
            public async Task<bool> PutMultimedia(MULTIMEDIA Multimedia)
            {
                context.multimedia.Update(Multimedia);
                await context.SaveChangesAsync();
                return true;
            }
            public async Task<bool> DeleteMultimedia(MULTIMEDIA Multimedia)
            {
                context.multimedia.Remove(Multimedia);
                await context.SaveChangesAsync();
                return true;
            }
        }
}

