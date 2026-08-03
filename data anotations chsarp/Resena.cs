using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace tablas_ecoscolombia
{
    public class Resena
    {
        [Key]
        public int idResena {  get; set; }
        
        public int idUsuario { get; set; }
        public int idLugar { get; set; }
        
        public string Comentario { get; set; }

        [System.ComponentModel.DataAnnotations.Range(1,5)]
        public int Calificacion { get; set; }
        public DateTime? Fecha { get; set; }

        [ForeignKey("idUsuario")]
        public Usuario Usuario { get; set; }

        [ForeignKey("idLugar")]
        public LugarTuristico Lugar { get; set; }
    }
}
