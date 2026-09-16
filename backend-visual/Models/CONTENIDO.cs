using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.models
{
    public class CONTENIDO
    {
        [Key]
        public int IdContenido { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        public int IdTipo { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        public DateTime FechaPublicacion { get; set; }
    }
}
