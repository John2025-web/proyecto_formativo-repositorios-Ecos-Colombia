using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.Models
{
    public class Tradicion
    {
        [Key]
        public int idTradicion { get; set; }

        public string Nombre { get; set; }
        public int idRegion { get; set; }
        public DateTime? FechaCelebracion { get; set; }

        public Region Region { get; set; }
    }
}
