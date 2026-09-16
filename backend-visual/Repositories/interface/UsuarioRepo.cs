using ApiPrueba.Models;

namespace ApiPrueba.Repositorios.Interfaces
{ 
    public interface IUsuarioRepo
    {
        Task<List<Usuario>> GetUsuario();
        Task<Usuario?> GetUsuarioById(int id);
        Task<bool> PostUsuario(Usuario usuario);
        Task<bool> PutUsuario(Usuario usuario);
        Task<bool> DeleteUsuario(Usuario usuario);

    }
}
