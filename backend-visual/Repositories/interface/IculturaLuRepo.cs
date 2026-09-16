using ApiPrueba.models;
using ApiPrueba.Models;
namespace ApiPrueba.Repositorios.Interfaces;

public interface IculturaLugarRepo
{
    Task<List<culturaLugar>> GetculturaLugar();

    Task<culturaLugar?> GetculturaLugarById(int id);

    Task<bool> PostculturaLugar(culturaLugar culturaLugar);

    Task<bool> PutculturaLugar(culturaLugar culturaLugar);

    Task<bool> DeleteculturaLugar(culturaLugar culturaLugar);
}