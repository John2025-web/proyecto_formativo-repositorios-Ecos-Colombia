using ApiPrueba.models;
using ApiPrueba.Models;
namespace ApiPrueba.Repositorios.Interfaces;

public interface IGastroRepo
{
    Task<List<GASTRONOMIA>> GetGastronomia();

    Task<GASTRONOMIA?> GetGastronomiaById(int id);

    Task<bool> PostGastronomia(GASTRONOMIA gastronomia);

    Task<bool> PutGastronomia(GASTRONOMIA  gastronomia);

    Task<bool> DeleteGastronomia(GASTRONOMIA gastronomia);
}
