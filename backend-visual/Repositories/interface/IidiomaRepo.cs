using ApiPrueba.models;
using ApiPrueba.Models;

namespace ApiPrueba.Repositorios.Interfaces
{
    public interface IidiomaRepo
    {
            Task<List<IDIOMA>> GetIDIOMA();

            Task<IDIOMA?> GetIDIOMAById(int id);

            Task<bool> PostIDIOMA(IDIOMA IDIOMA);

            Task<bool> PutIDIOMA(IDIOMA IDIOMA);

            Task<bool> DeleteIDIOMA(IDIOMA IDIOMA);
        }
    }
