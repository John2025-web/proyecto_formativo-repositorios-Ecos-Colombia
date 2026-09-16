using System.ComponentModel.DataAnnotations;

namespace ApiPrueba.models
{
    public class ADMINISTRADORES
    {
        [Key]
        public int IdAdmin { get; set; }

        [Required (ErrorMessage = "Campo Requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        public string Correo { get; set; }


    }
}
