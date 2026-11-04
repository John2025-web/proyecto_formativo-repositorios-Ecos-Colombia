using ApiPrueba.models;

namespace ApiPrueba.Repositories.@interface
{
    public interface ICULTURARepository
    {
        Task<List<CULTURA>> GetCulturas();
        Task<CULTURA> GetCulturaById(int id);
        Task<bool> PostCultura(CULTURA cultura);
        Task<bool> PutCultura(CULTURA cultura);
        Task<bool> DeleteCultura(CULTURA cultura);
    }
}