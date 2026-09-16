using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.models
{
    public class CULTURA
    {
        [Key]
        [Column("id_cultura")]
        public int IdCultura { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [Column("descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [Column("idCategoria")]
        public int IdCategoria { get; set; }
    }
}


