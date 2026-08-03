using System.ComponentModel.DataAnnotations;

namespace pruebaApi.models
{
    public class IDIOMA
    {
        [Key]
        public string CodigoIso {  get; set; }

        [Required(ErrorMessage = "campo Requerido")]
        public string IdiomaNombre { get; set; }


    }
}
