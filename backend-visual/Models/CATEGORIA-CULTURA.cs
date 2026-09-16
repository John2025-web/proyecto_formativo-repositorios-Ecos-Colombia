using System.ComponentModel.DataAnnotations;

namespace ApiPrueba.models
{
    public class CATEGORIA_CULTURA

    {
        [Key]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        public string Nombre { get; set; }

    }
}
