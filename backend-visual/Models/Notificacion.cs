using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.Models
{
    public class Notificacion
    {
        [Key]
        public int idNotificacion { get; set; }

        public int idUsuario { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Tipo { get; set; }

        [ForeignKey("idUsuario")]
        public Usuario Usuario { get; set; }
    }
}   
