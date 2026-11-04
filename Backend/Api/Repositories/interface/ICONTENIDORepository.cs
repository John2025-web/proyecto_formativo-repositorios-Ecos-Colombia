using ApiPrueba.models;

namespace ApiPrueba.Repositories.@interface
{
    public interface ICONTENIDORepository
    {
        Task<List<CONTENIDO>> GetContenidos();
        Task<CONTENIDO?> GetContenidoById(int id);
        Task<bool> PostContenido(CONTENIDO contenido);
        Task<bool> PutContenido(CONTENIDO contenido);
        Task<bool> DeleteContenido(CONTENIDO contenido);
    }
}