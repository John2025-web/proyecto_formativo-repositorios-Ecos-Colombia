using ApiPrueba.DBcontext;
using ApiPrueba.Models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace ApiPrueba.Repositorios
{
    public class RegionRepositoy : IRegionRepo
    {
            private readonly RegionService context;

            public RegionRepositoy(RegionService context)
            {
                this.context = context;
            }

            public async Task<List<Region>> GetRegion()
            {
                var data = await context.RegionServices.ToListAsync();
                return data;
            }

            public async Task<Region?> GetRegionById(int id)
            {
                var data = await context.RegionServices.FirstOrDefaultAsync(x => x.idRegion == id);
                return data;
            }

            public async Task<bool> PostRegion(Region region)
            {
                await context.RegionServices.AddAsync(region);
                await context.SaveAsync();
                return true;
            }

            public async Task<bool> PutRegion(Region region)
            {
                context.RegionServices.Update(region);
                await context.SaveAsync();
                return true;
            }

            public async Task<bool> DeleteRegion(Region region)
            {
                context.RegionServices.Remove(region);
                await context.SaveAsync();
                return true;
            }
        }
    }
