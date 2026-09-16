using ApiPrueba.Models;

namespace ApiPrueba.Repositorios.Interfaces
{
    public interface IRecenaRepo
    {
        Task<List<Recena>> GetRecena();
        Task<Recena?> GetRecenaById(int  id);
        Task<bool> PostRecena(Recena recena);
        Task<bool> PutRecena(Recena recena);
        Task<bool> DeleteRecena(Recena recena);
    }
}
