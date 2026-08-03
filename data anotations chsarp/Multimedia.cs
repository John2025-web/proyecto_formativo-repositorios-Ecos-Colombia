using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pruebaApi.models
{
    public class Multimedia
    {
        [Key]
        public int IdMultimedia { get; set; }
        
        [ForeignKey("id_contenido")]
        public int IdContenido { get; set; }

        [Required(ErrorMessage = "campo Requerido")]
        public string tipoContenido { get; set; }

        [Required(ErrorMessage = "campo Requerido")]
        public string url { get; set; }

    }
}
