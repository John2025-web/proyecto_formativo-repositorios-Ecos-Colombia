using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.models
{
    public class CONTENIDO_LUGAR
    {
        [Key]
        [ForeignKey("Id_Contenido")]
        public int IdContenido { get; set; }

        [Key]
        [ForeignKey("Id_Lugar")]
        public int IdLugar { get; set; }
    }
}