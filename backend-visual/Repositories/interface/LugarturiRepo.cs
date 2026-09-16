using ApiPrueba.models;
using ApiPrueba.repositorios;


namespace ApiPrueba.Repositorios.Interfaces
{
    public interface ILugarTuristicoRepo
    {
        Task<List<LUGAR_TURISTICO>> GetLugarTuristico();

        Task<LUGAR_TURISTICO?> GetLugarTuristicoById(int id);

        Task<bool> PostLugarTuristico(LUGAR_TURISTICO LugarTuristico);

        Task<bool> PutLugarTuristico(LUGAR_TURISTICO LugarTuristico);

        Task<bool> DeleteLugarTuristico(LUGAR_TURISTICO LugarTuristico);
    }
}