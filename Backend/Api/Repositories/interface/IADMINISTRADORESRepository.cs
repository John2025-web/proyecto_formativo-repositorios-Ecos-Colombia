using ApiPrueba.models;

namespace ApiPrueba.Repositories.@interface
{
    public interface IADMINISTRADORESRepository
    {
        Task<List<ADMINISTRADORES>> GetAdministradores();

        Task<ADMINISTRADORES?> GetAdministradorById(int id);

        Task<bool> PostAdministrador(ADMINISTRADORES admin);

        Task<bool> PutAdministrador(ADMINISTRADORES admin);

        Task<bool> DeleteAdministrador(ADMINISTRADORES admin);
    }
}
