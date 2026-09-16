using ApiPrueba.models;
using ApiPrueba.Models;

namespace ApiPrueba.Repositorios.Interfaces
{
    public interface IMultimediaRepo
    {
        Task<List<MULTIMEDIA>> GetMultimedia();

        Task<MULTIMEDIA?> GetMultimediaById(int id);

        Task<bool> PostMultimedia(MULTIMEDIA Multimedia);

        Task<bool> PutMultimedia(MULTIMEDIA  Multimedia);

        Task<bool> DeleteMultimedia(MULTIMEDIA Multimedia);
    }
}
