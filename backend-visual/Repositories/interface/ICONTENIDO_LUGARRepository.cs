using ApiPrueba.models;

namespace ApiPrueba.Repositories.@interface
{
    public interface ICONTENIDO_LUGARRepository
    {
        Task<List<CONTENIDO_LUGAR>> GetContenidoLugar();
        Task<CONTENIDO_LUGAR> GetContenidoLugarById(int idContenido, int idLugar);
        Task<bool> PostContenidoLugar(CONTENIDO_LUGAR contenidoLugar);
        Task<bool> PutContenidoLugar(CONTENIDO_LUGAR contenidoLugar);
        Task<bool> DeleteContenidoLugar(CONTENIDO_LUGAR contenidoLugar);
    }
}