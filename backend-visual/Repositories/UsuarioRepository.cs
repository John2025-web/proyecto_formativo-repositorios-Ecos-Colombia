using ApiPrueba.DBcontext;
using ApiPrueba.Models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace ApiPrueba.Repositorios
{
    public class UsuarioRepository : IUsuarioRepo
    {
        private readonly UsuarioService context;

        public UsuarioRepository(UsuarioService context)
        {
            this.context = context;
        }

        public async Task<List<Usuario>> GetUsuario()
        {
            var data = await context.usuario.ToListAsync();
            return data;
        }

        public async Task<Usuario?> GetUsuarioById(int id)
        {
            var data = await context.usuario.FirstOrDefaultAsync(x => x.idUsuario == id);
            return data;
        }

        public async Task<bool> PostUsuario(Usuario usuario)
        {
            await context.usuario.AddAsync(usuario);
            await context.SaveAsync();
            return true;
        }

        public async Task<bool> PutUsuario(Usuario usuario)
        {
            context.usuario.Update(usuario);
            await context.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteUsuario(Usuario usuario)
        {
            context.usuario.Remove(usuario);
            await context.SaveAsync();
            return true;
        }
    }
}
