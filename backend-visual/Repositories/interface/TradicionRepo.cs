using ApiPrueba.Models;

namespace ApiPrueba.Repositorios.Interfaces
{
    public interface ITradicionRepo
    {
        public Task<List<Tradicion>> GetTradicion();
        public Task<Tradicion?> GetTradicionById(int Id);
        public Task<bool> PostTradicion(Tradicion tradicion);
        public Task<bool> PutTradicion(Tradicion tradicion);
        public Task<bool> DeleteTradicion(Tradicion tradicion);
    }
}
