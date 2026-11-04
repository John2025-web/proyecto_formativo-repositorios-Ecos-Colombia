using ApiPrueba.models;
using ApiPrueba.Models;
using ApiPrueba.repositorios;

namespace ApiPrueba.Repositorios.Interfaces
{
    public interface IculturaTradicionRepo
    {
        Task<List<culturaTradicion>> GetculturaTradicion();

        Task<culturaTradicion?> GetculturaTradicionById(int id);

        Task<bool> PostculturaTradicion(culturaTradicion culturaTradicion);

        Task<bool> PutculturaTradicion(culturaTradicion culturaTradicion);

        Task<bool> DeleteculturaTradicion(culturaTradicion culturaTradicion);
    }
}
