using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pruebaApi.models
{
    public class culturaLugar
    {
        [Key]
        [ForeignKey("id_cultura")]
        public int idCultura {  get; set; }

        [Key]
        [ForeignKey("id_lugar")]
        public int idLugar { get; set; }
    
    }
}
