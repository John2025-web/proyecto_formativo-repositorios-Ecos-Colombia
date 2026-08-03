using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pruebaApi.models
{
    public class culturaTradicion
    {
        [Key]
        [ForeignKey("id_cultura")]
        public int idCultura {  get; set; }
        [Key]
        [ForeignKey("id_tradicion")]
        public int idTradicion { get; set; }

    }
}
