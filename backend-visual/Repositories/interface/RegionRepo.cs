using ApiPrueba.Models;

namespace ApiPrueba.Repositorios.Interfaces
{
    public interface IRegionRepo
    {
        Task<List<Region>> GetRegion();
        Task<Region?> GetRegionById(int id);
        Task<bool> PostRegion(Region region);
        Task<bool> PutRegion(Region region);
        Task<bool> DeleteRegion(Region region);
    }
}