using ApiPrueba.models;

namespace ApiPrueba.Repositories.@interface
{
    public interface ICONTENIDO_CULTURARepository
    {
        Task<List<CONTENIDO_CULTURA>> GetContenidoCultura();
        Task<CONTENIDO_CULTURA> GetContenidoCulturaById(int idContenido, int idCultura);
        Task<bool> PostContenidoCultura(CONTENIDO_CULTURA contenidoCultura);
        Task<bool> PutContenidoCultura(CONTENIDO_CULTURA contenidoCultura);
        Task<bool> DeleteContenidoCultura(CONTENIDO_CULTURA contenidoCultura);
    }
}