using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.models
{
    [Table("CONTENIDO_CULTURA")]
    public class CONTENIDO_CULTURA
    {
        [Column("id_contenido")]
        public int IdContenido { get; set; }

        [Column("id_cultura")]
        public int IdCultura { get; set; }
    }
}