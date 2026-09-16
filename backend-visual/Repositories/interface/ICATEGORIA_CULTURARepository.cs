using ApiPrueba.models;

namespace ApiPrueba.Repositories.@interface
{
    public interface ICATEGORIA_CULTURARepository
    {
        Task<List<CATEGORIA_CULTURA>> GetCategorias();
        Task<CATEGORIA_CULTURA> GetCategoriaById(int id);
        Task<bool> PostCategoria(CATEGORIA_CULTURA categoria);
        Task<bool> PutCategoria(CATEGORIA_CULTURA categoria);
        Task<bool> DeleteCategoria(CATEGORIA_CULTURA categoria);
    }
}
