using ApiPrueba.DBcontext;
using ApiPrueba.Models;
using ApiPrueba.Repositorios;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Repositorios
{
    public class NotificacionRepository : INotificacionRepo
    {
        private readonly NotificacionService context; 

        public NotificacionRepository(NotificacionService context)
        {
            this.context = context;
        }

        public async Task<List<Notificacion>> GetNotificacion()
        {
            var data = await context.Notificacion.ToListAsync();
            return data;
        }

        public async Task<Notificacion?> GetNotificacionById(int id)
        {
            var data = await context.Notificacion.FirstOrDefaultAsync(x => x.idNotificacion == id);
            return data;
        }

        public async Task<bool> PostNotificacion(Notificacion notificacion)
        {
            await context.Notificacion.AddAsync(notificacion);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PutNotificacion(Notificacion notificacion)
        {
            context.Notificacion.Update(notificacion);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotificacion(Notificacion notificacion)
        {
            context.Notificacion.Remove(notificacion);
            await context.SaveChangesAsync();
            return true;
        }
    }
}