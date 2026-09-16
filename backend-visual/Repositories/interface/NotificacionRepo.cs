using ApiPrueba.Models;

namespace ApiPrueba.Repositorios.Interfaces
{
    public interface INotificacionRepo
    {
        Task<List<Notificacion>> GetNotificacion();
        Task<Notificacion> GetNotificacionById(int id);
        Task<bool> PostNotificacion(Notificacion notificacion);
        Task<bool> PutNotificacion(Notificacion notificacion);
        Task<bool> DeleteNotificacion(Notificacion notificacion);
    }
}   