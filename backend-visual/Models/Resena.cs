using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.Models
{
    public class Recena
    {
        [Key]
        public int idRecena { get; set; }

        public int idUsuario { get; set; }
        public int idLugar { get; set; }

        public string Comentario { get; set; }

        [Range(1, 5)]
        public int Calificacion { get; set; }

        public DateTime? Fecha { get; set; }

        [ForeignKey("idUsuario")]
        public Usuario Usuario { get; set; }

        [ForeignKey("idLugar")]
        public string LugarTuristico { get; set; }
    }
}
