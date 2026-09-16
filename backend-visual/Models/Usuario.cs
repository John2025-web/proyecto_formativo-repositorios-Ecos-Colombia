using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.Models;
    public class Usuario
    {
        public int idUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string PaisOrigen { get; set; }
        public string IdiomaPreferido { get; set; }
        public string TipoUsuario { get; set; }
        public string PasswordHash { get; set; }

}

