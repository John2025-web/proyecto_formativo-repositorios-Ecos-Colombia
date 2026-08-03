using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pruebaApi.models
{
    public class Gastronomia
    {
        [Key]
        public int IdPlato { get; set; }

        [Required(ErrorMessage ="campo Requerido")]
        public string Nombre { get; set; }

        [ForeignKey("id_region")]
        public int IdRegion { get; set; }

        [Required(ErrorMessage = "campo Requerido")]
        public string Ingredientes  { get; set; }

        [Required(ErrorMessage = "campo Requerido")]
        public string DescripcionPlato { get; set; }

        [Required(ErrorMessage = "campo Requerido")]
        public string  ImagenPlato  { get; set; }
    }
}
