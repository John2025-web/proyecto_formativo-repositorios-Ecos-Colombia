using System.ComponentModel.DataAnnotations;

namespace ApiPrueba.models
{
    public class CULTURA
    {
        [Key]
        public int IdCultura { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        public int IdCategoria { get; set; }

    }
}
