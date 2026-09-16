using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.Models
{
    public class Region
    {
        [Key]
        public int idRegion { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        public List<Tradicion> Tradiciones { get; set; }
    }
}
