using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.models
{
    public class CONTENIDO_CULTURAL
    {

        [Key]
        [ForeignKey("Id_Contenido")]
        public int IdContenido { get; set; }

        [Key]
        [ForeignKey("Id_Cultura")]
        public int IdCultura { get; set; }
    }
}
