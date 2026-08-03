using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tablas_ecoscolombia
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
        public Usuario Usuario {get; set;}

        
    }
}
