using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.Models
{
    public class Tipo_Contenido
    {

        [Key]
        public int idTipo { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }
    }
}
