using ApiPrueba.Models;

namespace ApiPrueba.Repositorios.Interfaces
{
    public interface ITipo_ContenidoRepo
    {
        Task<List<Tipo_Contenido>> GetTipo_Contenido();
        Task<Tipo_Contenido?> GetTipo_ContenidoById(int id);
        Task<bool> PostTipo_Contenido(Tipo_Contenido tipo_Contenido);
        Task<bool> PutTipo_Contenido(Tipo_Contenido tipo_Contenido);
        Task<bool> DeleteTipo_Contenido(Tipo_Contenido tipo_Contenido);
    }
}
