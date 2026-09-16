using ApiPrueba.DBcontex;
using ApiPrueba.models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using static ApiPrueba.DBcontex.interfaceGastroRepo;

namespace ApiPrueba.repositorios;

public class gastroRepository : IGastroRepo
{
    private readonly GastronomiaService context;

    public gastroRepository(GastronomiaService context)
    {
        this.context = context;
    }
    public async Task<List<GASTRONOMIA>> GetGastronomia()
    {
        var data = await context.Gastronomia.ToListAsync();
        return data;
    }
    public async Task<GASTRONOMIA?> GetGastronomiaById(int id)
    {
        var data = await context.Gastronomia.FirstOrDefaultAsync(X => X.IdPlato == id);
        return data;
    }
    public async Task<bool> PostGastronomia(GASTRONOMIA gastronomia)
    {
        await context.Gastronomia.AddAsync(gastronomia);
        await context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> PutGastronomia(GASTRONOMIA gastronomia)
    {
        context.Gastronomia.Update(gastronomia);
        await context.SaveAsync();
        return true;
    }
    public async Task<bool> DeleteGastronomia(GASTRONOMIA gastronomia)
    {
        context.Gastronomia.Remove(gastronomia);
        await context.SaveAsync();
        return true;
    }
}
