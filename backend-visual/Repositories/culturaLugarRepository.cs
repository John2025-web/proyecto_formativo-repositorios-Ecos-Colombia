using ApiPrueba.DBcontex;
using ApiPrueba.models;
using ApiPrueba.Repositories.@interface;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.repositorios;

public class CulturaLugarRepository : IculturaLugarRepo
{
    private readonly cultura_LugarService context;

    public CulturaLugarRepository(cultura_LugarService context)
    {
        this.context = context;
    }
    public async Task<List<culturaLugar>> GetculturaLugar()
    {
        var data = await context.cultura_Lugar.ToListAsync(); 
        return data;
    }
    public async Task<culturaLugar?> GetculturaLugarById(int id)
    {
        var data = await context.cultura_Lugar.FirstOrDefaultAsync(X => X.idCultura == id);
        return data;
    }
    public async Task<bool> PostculturaLugar(culturaLugar culturaLugar)
    {
        await context.cultura_Lugar.AddAsync(culturaLugar);
        await context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> PutculturaLugar(culturaLugar culturaLugar)
    {
        context.cultura_Lugar.Update(culturaLugar);
        await context.SaveAsync();
        return true;
    }
    public async Task<bool> DeleteculturaLugar(culturaLugar culturaLugar)
    {
        context.cultura_Lugar.Remove(culturaLugar);
        await context.SaveAsync();
        return true;
    }

    public Task<List<CULTURA>> GetCulturas()
    {
        throw new NotImplementedException();
    }

    public Task<CULTURA> GetCulturaById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> PostCultura(CULTURA cultura)
    {
        throw new NotImplementedException();
    }

    public Task<bool> PutCultura(CULTURA cultura)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteCultura(CULTURA cultura)
    {
        throw new NotImplementedException();
    }
}
